using Tel.Core.Client;
using Tel.Core.Extensions;
using Tel.Core.Models;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Yarp.ReverseProxy.Forwarder;

namespace Tel.Core.Forwarder;

public class TelForwarderHttpClientFactory : ForwarderHttpClientFactory
{
    readonly ILogger<TelForwarderHttpClientFactory> logger;
    readonly TelCoreServer TelServer;
    private readonly IHttpContextAccessor _httpContextAccessor;
    static int connectionCount;

    public TelForwarderHttpClientFactory(
        ILogger<TelForwarderHttpClientFactory> logger,
        IHttpContextAccessor httpContextAccessor, TelCoreServer TelServer)
    {
        this.TelServer = TelServer;
        this.logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override void ConfigureHandler(ForwarderHttpClientContext context, SocketsHttpHandler handler)
    {
        base.ConfigureHandler(context, handler);
        handler.ConnectCallback = ConnectCallback;
    }

    private async ValueTask<Stream> ConnectCallback(SocketsHttpConnectionContext context, CancellationToken cancellationToken)
    {
        var host = context.InitialRequestMessage.RequestUri.Host;

        var contextRequest = _httpContextAccessor.HttpContext;
        //var lifetime = contextRequest.Features.Get<IConnectionLifetimeFeature>()!;

        try
        {
            Interlocked.Increment(ref connectionCount);
            var res = await proxyAsync(host, context, contextRequest.RequestAborted);
            return res;
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            Interlocked.Decrement(ref connectionCount);
            logger.LogDebug($"统计YARP连接数：{connectionCount}");
        }
    }

    public async ValueTask<Stream> proxyAsync(string host, SocketsHttpConnectionContext context, CancellationToken cancellation)
    {
        WebInfo web;
        if (!TelServer.WebList.TryGetValue(host, out web))
        {
            // 客户端已离线
            return await OfflinePage(host, context);
        }

        var msgId = Guid.NewGuid().ToString().Replace("-", "");

        TaskCompletionSource<Stream> tcs = new();
        logger.LogDebug($"[Http]Swap开始 {msgId}|{host}=>{web.WebConfig.LocalIp}:{web.WebConfig.LocalPort}");

        cancellation.Register(() =>
        {
            logger.LogDebug($"[Proxy TimeOut]:{msgId}");
            tcs.TrySetCanceled();
        });

        TelServer.ResponseTasks.TryAdd(msgId, (tcs, cancellation));

        try
        {
            // 发送指令给客户端，等待建立隧道
            await web.Socket.SendCmdAsync(MessageType.SwapMsg, $"{msgId}|{web.WebConfig.LocalIp}:{web.WebConfig.LocalPort}", cancellation);
            var res = await tcs.Task.WaitAsync(cancellation);

            logger.LogDebug($"[Http]Swap OK {msgId}");
            return res;
        }
        catch (WebSocketException)
        {
            // 通讯异常，返回客户端离线
            return await OfflinePage(host, context);
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            TelServer.ResponseTasks.TryRemove(msgId, out _);
        }
    }


    private async ValueTask<Stream> OfflinePage(string host, SocketsHttpConnectionContext context)
    {
        var bytes = Encoding.UTF8.GetBytes(
            $"HTTP/1.1 200 OK\r\nContent-Type:text/html; charset=utf-8\r\n\r\n{TunnelResource.Page_Offline}\r\n");

        return await Task.FromResult(new ResponseStream(bytes));
    }
}
