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
using System.Threading;
using System.Threading.Tasks;
using FastTunnel.Core.Client;
using FastTunnel.Core.Extensions;
using FastTunnel.Core.Handlers.Server;
using FastTunnel.Core.Utilitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

namespace FastTunnel.Core.Forwarder.MiddleWare;
public class CheckWebAllowAccessIpsHandler
{
    readonly ILogger<CheckWebAllowAccessIpsHandler> logger;
    readonly FastTunnelServer fastTunnelServer;
    readonly ILoginHandler loginHandler;

    public CheckWebAllowAccessIpsHandler(ILogger<CheckWebAllowAccessIpsHandler> logger, FastTunnelServer fastTunnelServer, ILoginHandler loginHandler)
    {
        this.logger = logger;
        this.fastTunnelServer = fastTunnelServer;
        this.loginHandler = loginHandler;
    }
    public async Task Handle(HttpContext context, Func<Task> next)
    {
        if (fastTunnelServer.ServerOption.CurrentValue.WebAllowAccessIps.Length > 0)
        {
            var requestIp = context.Connection.RemoteIpAddress.GetIPV4Address();
            var clientsIps = fastTunnelServer.Clients.Select(t => t.RemoteIpAddress.GetIPV4Address());
            var webIps = fastTunnelServer.ServerOption.CurrentValue.WebAllowAccessIps.Select(t => IPAddress.Parse(t));
            if (clientsIps.Contains(requestIp) || webIps.Contains(requestIp))
            {
                Console.WriteLine($"IP Web Flite :{requestIp} open");
                await next();
            }
            else
            {
                Console.WriteLine($"IP Web Flite :{requestIp} close");
                logger.LogDebug($"【{context.Request.GetDisplayUrl()}】: Close Accept");
                await context.Response.WriteAsync($"<p> {requestIp} Access Denied</p>");
            }
        }
        else
        {
            await next();
        }
    }
}
