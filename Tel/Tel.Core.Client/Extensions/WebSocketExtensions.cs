using Tel.Core.Exceptions;
using Tel.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tel.Core.Extensions   
{
    public static class WebSocketExtensions
    {
        public static async Task SendCmdAsync(this WebSocket socket, MessageType type, string content, CancellationToken cancellationToken)
        {
            if (socket.State == WebSocketState.Closed || socket.State == WebSocketState.Aborted)
            {
                throw new SocketClosedException(socket.State.ToString());
            }

            var buffer = Encoding.UTF8.GetBytes($"{(char)type}{content}\n");
            await socket.SendAsync(buffer, WebSocketMessageType.Binary, false, cancellationToken);
        }
    }
}
