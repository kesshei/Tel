using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Tel.Core.Models
{
    public class ForwardHandlerArg
    {
        public ForwardConfig SSHConfig { get; set; }

        public Socket LocalClient { get; set; }
    }
}
