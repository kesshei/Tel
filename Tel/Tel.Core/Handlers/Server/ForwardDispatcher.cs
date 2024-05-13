using Tel.Core.Client;
using Tel.Core.Exceptions;
using Tel.Core.Extensions;
using Tel.Core.Listener;
using Tel.Core.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tel.Core.Handlers.Server
{
    public class ForwardDispatcher
    {
        private TelCoreServer _server;
        private ForwardConfig _config;
        ILogger logger;

        public ForwardDispatcher(ILogger logger, TelCoreServer server, ForwardConfig config)
        {
            this.logger = logger;
            _server = server;
            _config = config;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_socket">用户请求</param>
        /// <param name="client">FastTunnel客户端</param>
        /// <returns></returns>
        public async Task DispatchAsync(Socket _socket, WebSocket client, PortProxyListener listener)
        {
            var msgId = Guid.NewGuid().ToString().Replace("-", "");

            try
            {
                await Task.Yield();
                logger.LogDebug($"[Forward]Swap开始 {msgId}|{_config.RemotePort}=>{_config.LocalIp}:{_config.LocalPort}");

                var tcs = new TaskCompletionSource<Stream>();

                _server.ResponseTasks.TryAdd(msgId, (tcs, CancellationToken.None));

                try
                {
                    await client.SendCmdAsync(MessageType.Forward, $"{msgId}|{_config.LocalIp}:{_config.LocalPort}", CancellationToken.None);
                }
                catch (SocketClosedException sex)
                {
                    // TODO:客户端已掉线，但是没有移除对端口的监听
                    logger.LogError($"[Forward]Swap 客户端已离线 {sex.Message}");
                    tcs.TrySetCanceled();
                    Close(_socket);
                    return;
                }
                catch (Exception ex)
                {
                    // 网络不稳定
                    logger.LogError(ex, $"[Forward]Swap Exception");
                    tcs.TrySetCanceled();
                    Close(_socket);
                    return;
                }

                using (var stream1 = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(10)))
                using (var stream2 = new NetworkStream(_socket, true) { ReadTimeout = 1000 * 60 * 10 })
                {
                    await Task.WhenAny(stream1.CopyToAsync(stream2), stream2.CopyToAsync(stream1));
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug($"[Forward]Swap Error {msgId}：" + ex.Message);
            }
            finally
            {
                logger.LogDebug($"[Forward]Swap结束 {msgId}");
                _server.ResponseTasks.TryRemove(msgId, out _);
                listener.DecrementClients();
            }
        }

        private void Close(Socket socket)
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception)
            {
            }
            finally
            {
                socket.Close();
            }
        }
    }
}
