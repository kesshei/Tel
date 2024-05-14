using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tel.Core.Client;
using Tel.Core.Extensions;
using Tel.Core.Handlers.Server;
using Tel.Core.Utilitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

namespace Tel.Core.Forwarder.MiddleWare;
public class CheckWebAllowAccessIpsHandler
{
    readonly ILogger<CheckWebAllowAccessIpsHandler> logger;
    readonly TelCoreServer TelServer;
    readonly ILoginHandler loginHandler;

    public CheckWebAllowAccessIpsHandler(ILogger<CheckWebAllowAccessIpsHandler> logger, TelCoreServer TelServer, ILoginHandler loginHandler)
    {
        this.logger = logger;
        this.TelServer = TelServer;
        this.loginHandler = loginHandler;
    }
    public async Task Handle(HttpContext context, Func<Task> next)
    {
        //if (TelServer.ServerOption.CurrentValue.WebAllowAccessIps.Length > 0)
        //{
        //    var requestIp = context.Connection.RemoteIpAddress.GetIPV4Address();
        //    var clientsIps = TelServer.Clients.Select(t => t.RemoteIpAddress.GetIPV4Address());
        //    var webIps = TelServer.ServerOption.CurrentValue.WebAllowAccessIps.Select(t => IPAddress.Parse(t));
        //    if (clientsIps.Contains(requestIp) || webIps.Contains(requestIp))
        //    {
        //        Console.WriteLine($"IP Web Flite :{requestIp} open");
        //        await next();
        //    }
        //    else
        //    {
        //        Console.WriteLine($"IP Web Flite :{requestIp} close");
        //        logger.LogDebug($"【{context.Request.GetDisplayUrl()}】: Close Accept");
        //        await context.Response.WriteAsync($"<p> {requestIp} Access Denied</p>");
        //    }
        //}
        //else
        {
            await next();
        }
    }
}
