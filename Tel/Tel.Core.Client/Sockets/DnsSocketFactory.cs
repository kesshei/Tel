using Tel.Core.Extensions;
using Tel.Core.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Tel.Core.Client.Sockets
{

    public class DnsSocketFactory
    {
        public static async Task<Socket> ConnectAsync(string host, int port)
        {
            var Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var dnsEndPoint = new DnsEndPoint(host, port);
            await Socket.ConnectAsync(dnsEndPoint);
            return Socket;
        }
    }

}
