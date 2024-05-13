using Tel.Core.Client;
using Tel.Core.Models;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Tel.Core.Handlers.Server
{
    public interface IClientMessageHandler
    {
        bool NeedRecive { get; }

        Task<bool> HandlerMsg(TelCoreServer server, WebSocket client, string msg);
    }
}
