using Tel.Core.Client;
using Tel.Core.Handlers.Server;
using Tel.Core.Utilitys;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;

namespace Tel.Core.Listener
{
    public class PortProxyListener
    {
        readonly ILogger _logerr;

        public string ListenIp { get; set; }

        public int ListenPort { get; set; }

        int m_numConnectedSockets;

        bool shutdown;
        ForwardDispatcher _requestDispatcher;
        readonly Socket listenSocket;
        readonly WebSocket client;
        readonly TelCoreServer server;

        public PortProxyListener(string ip, int port, ILogger logerr, WebSocket client, TelCoreServer server)
        {
            this.client = client;
            _logerr = logerr;
            this.ListenIp = ip;
            this.ListenPort = port;
            this.server = server;

            IPAddress ipa = IPAddress.Parse(ListenIp);
            IPEndPoint localEndPoint = new IPEndPoint(ipa, ListenPort);

            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(localEndPoint);
        }

        public void Start(ForwardDispatcher requestDispatcher)
        {
            shutdown = false;
            _requestDispatcher = requestDispatcher;

            listenSocket.Listen();

            StartAccept(null);
        }
        private bool CheckForwardAllowAccessIps(Socket socket)
        {
            if (server.ServerOption.CurrentValue.ForwardAllowAccessIps.Length > 0)
            {
                var remoteAddress = (socket.RemoteEndPoint as IPEndPoint).Address.GetIPV4Address();
                var ips = server.ServerOption.CurrentValue.ForwardAllowAccessIps.Select(t => IPEndPoint.Parse(t).Address);
                var result= ips.Contains(remoteAddress);
                if (result)
                {
                    Console.WriteLine($"IP Forward Flite :{remoteAddress} open");
                }
                else
                {
                    Console.WriteLine($"IP Forward Flite :{remoteAddress} close");
                }
                return result;
            }
            return true;
        }
        private void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            try
            {
                _logerr.LogDebug($"【{ListenIp}:{ListenPort}】: StartAccept");
                if (acceptEventArg == null)
                {
                    acceptEventArg = new SocketAsyncEventArgs();
                    acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
                }
                else
                {
                    // socket must be cleared since the context object is being reused
                    acceptEventArg.AcceptSocket = null;
                }

                bool willRaiseEvent = listenSocket.AcceptAsync(acceptEventArg);
                if (!willRaiseEvent)
                {
                    ProcessAcceptAsync(acceptEventArg);
                }
            }
            catch (Exception ex)
            {
                _logerr.LogError(ex, "待处理异常");
            }
        }

        private async void ProcessAcceptAsync(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                var accept = e.AcceptSocket;
                if (CheckForwardAllowAccessIps(accept))
                {
                    IncrementClients();

                    // 将此客户端交由Dispatcher进行管理
                    _requestDispatcher.DispatchAsync(accept, client, this);

                }
                else
                {
                    try
                    {
                        accept?.Close();
                        _logerr.LogDebug($"【{ListenIp}:{ListenPort}】: Close Accept");
                    }
                    catch (Exception)
                    {
                    }
                }

                // Accept the next connection request
                StartAccept(e);
            }
            else
            {
                Stop();
            }
        }

        private void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAcceptAsync(e);
        }

        public void Stop()
        {
            if (shutdown)
                return;

            try
            {
                if (listenSocket.Connected)
                {
                    listenSocket.Shutdown(SocketShutdown.Both);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                shutdown = true;
                listenSocket.Close();
            }
        }

        internal void IncrementClients()
        {
            Interlocked.Increment(ref m_numConnectedSockets);
            _logerr.LogInformation($"[Listener:{ListenPort}] Accepted. There are {{0}} clients connected", m_numConnectedSockets);

        }

        internal void DecrementClients()
        {
            Interlocked.Decrement(ref m_numConnectedSockets);
            _logerr.LogInformation($"[Listener:{ListenPort}] DisConnet. There are {{0}} clients connecting", m_numConnectedSockets);

        }
    }
}
