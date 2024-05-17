using Tel.Core.Client;
using Tel.Core.Extensions;
using Tel.Core.Handlers.Server;
using Tel.Core.Models;
using Tel.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Tel.Core.Config;
using Tel.Core.Utilitys;

namespace Tel.Core.Forwarder.MiddleWare
{
    public class TelClientHandler
    {
        readonly ILogger<TelClientHandler> logger;
        readonly TelCoreServer TelServer;
        readonly Version serverVersion;
        readonly ILoginHandler loginHandler;

        static int connectionCount;

        public static int ConnectionCount => connectionCount;

        public TelClientHandler(
            ILogger<TelClientHandler> logger, TelCoreServer TelServer, ILoginHandler loginHandler)
        {
            this.logger = logger;
            this.TelServer = TelServer;
            this.loginHandler = loginHandler;

            serverVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        }

        public async Task Handle(HttpContext context, Func<Task> next)
        {
            try
            {
                if (!context.WebSockets.IsWebSocketRequest || !context.Request.Headers.TryGetValue(TelConst.TEL_VERSION, out var version))
                {
                    await next();
                    return;
                };

                Interlocked.Increment(ref connectionCount);

                try
                {
                    await handleClient(context, version);
                }
                finally
                {
                    Interlocked.Decrement(ref connectionCount);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
            }
        }

        private async Task handleClient(HttpContext context, string clientVersion)
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

            if (Version.Parse(clientVersion).Major != serverVersion.Major)
            {
                await Close(webSocket, $"客户端版本{clientVersion}与服务端版本{serverVersion}不兼容，请升级。");
                return;
            }

            if (!checkToken(context))
            {
                await Close(webSocket, "Token验证失败");
                return;
            }

            var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
            var log = loggerFactory.CreateLogger<TunnelClient>();
            var client = new TunnelClient(webSocket, TelServer, loginHandler, context.Connection.RemoteIpAddress, log);
            client.ConnectionPort = context.Connection.LocalPort;

            try
            {
                TelServer.ClientLogin(client);
                await client.ReviceAsync(context.RequestAborted);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "客户端异常");
            }
            finally
            {
                TelServer.ClientLogout(client);
            }
        }

        private static async Task Close(WebSocket webSocket, string reason)
        {
            await webSocket.SendCmdAsync(MessageType.Log, reason, CancellationToken.None);
            await webSocket.CloseAsync(WebSocketCloseStatus.Empty, string.Empty, CancellationToken.None);
            return;
        }

        private bool checkToken(HttpContext context)
        {
            var checkToken = false;
            if (TelServer.ServerOption.CurrentValue.Tokens != null && TelServer.ServerOption.CurrentValue.Tokens.Count != 0)
            {
                checkToken = true;
            }

            if (!checkToken)
                return true;

            // 客户端未携带token，登录失败
            if (!context.Request.Headers.TryGetValue(TelConst.TEL_TOKEN, out var token))
                return false;

            if (TelServer.ServerOption.CurrentValue.Tokens.Select(t => t.Hash512()).ToList().Contains(token))
            {
                var requestIp = context.Connection.RemoteIpAddress.GetIPV4Address();
                SystemConfig.IpInfos.ReRead();
                SystemConfig.IpInfos.CurrentConfig.disIps.Remove(requestIp.ToString());
                if (!SystemConfig.IpInfos.CurrentConfig.Ips.Contains(requestIp.ToString()))
                {
                    SystemConfig.IpInfos.CurrentConfig.Ips.Add(requestIp.ToString());
                }
                SystemConfig.IpInfos.Save();

                return true;
            }

            return false;
        }
    }
}
