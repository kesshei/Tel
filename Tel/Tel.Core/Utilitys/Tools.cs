using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tel.Core.Utilitys;
public static class Tools
{
    public static IPAddress GetIPV4Address(this IPAddress iPAddress)
    {
        if (iPAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            return iPAddress.MapToIPv4();
        }
        return iPAddress;
    }
}
