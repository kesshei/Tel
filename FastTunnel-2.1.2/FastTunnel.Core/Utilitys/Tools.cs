// Licensed under the Apache License, Version 2.0 (the "License").
// You may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//     https://github.com/FastTunnel/FastTunnel/edit/v2/LICENSE
// Copyright (c) 2019 Gui.H

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FastTunnel.Core.Utilitys;
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
