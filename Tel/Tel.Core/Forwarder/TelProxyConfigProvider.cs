using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarp.ReverseProxy.Configuration;

namespace Tel.Core.Forwarder
{
    public class TelProxyConfigProvider : IProxyConfigProvider
    {
        public IProxyConfig GetConfig()
        {
            return new TelProxyConfig();
        }
    }
}
