using Tel.Core.Config;
using Tel.Core.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Options;
using System.IO;
using Yarp.ReverseProxy.Configuration;
using System.Collections.Generic;
using Tel.Core.Forwarder.MiddleWare;

namespace Tel.Core.Client
{
    public class TelCoreServer
    {
        public int ConnectedClientCount;
        public readonly IOptionsMonitor<DefaultServerConfig> ServerOption;
        public IProxyConfigProvider proxyConfig;
        readonly ILogger<TelCoreServer> logger;

        public ConcurrentDictionary<string, (TaskCompletionSource<Stream>, CancellationToken)> ResponseTasks { get; } = new();

        public ConcurrentDictionary<string, WebInfo> WebList { get; private set; } = new();

        public ConcurrentDictionary<int, ForwardInfo<ForwardHandlerArg>> ForwardList { get; private set; }
            = new ConcurrentDictionary<int, ForwardInfo<ForwardHandlerArg>>();

        /// <summary>
        /// 在线客户端列表
        /// </summary>
        public IList<TunnelClient> Clients = new List<TunnelClient>();

        public TelCoreServer(ILogger<TelCoreServer> logger, IProxyConfigProvider proxyConfig, IOptionsMonitor<DefaultServerConfig> serverSettings)
        {
            this.logger = logger;
            this.ServerOption = serverSettings;
            this.proxyConfig = proxyConfig;
        }

        /// <summary>
        /// 客户端登录
        /// </summary>
        /// <param name="client"></param>
        internal void ClientLogin(TunnelClient client)
        {
            Interlocked.Increment(ref ConnectedClientCount);
            logger.LogInformation($"客户端连接 {client.RemoteIpAddress} 当前在线数：{ConnectedClientCount}，统计CLIENT连接数：{TelClientHandler.ConnectionCount}");
            Clients.Add(client);
        }

        /// <summary>
        /// 客户端退出
        /// </summary>
        /// <param name="client"></param>
        /// <exception cref="NotImplementedException"></exception>
        internal void ClientLogout(TunnelClient client)
        {
            Interlocked.Decrement(ref ConnectedClientCount);
            logger.LogInformation($"客户端关闭  {client.RemoteIpAddress} 当前在线数：{ConnectedClientCount}，统计CLIENT连接数：{TelClientHandler.ConnectionCount}");
            Clients.Remove(client);
            client.Logout();
        }
    }
}
