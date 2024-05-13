using Tel.Core.Models;
using Tel.Core.Models.Massage;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Tel.Core.Extensions
{
    public static class SocketExtensions
    {
        public static void SendCmd<T>(this Socket socket, Message<T> message)
            where T : TunnelMassage
        {
            socket.Send(Encoding.UTF8.GetBytes(message.ToJson() + "\n"));
        }
    }
}
