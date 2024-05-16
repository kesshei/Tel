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
using Tel.Core.Config;

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
        SystemConfig.IpInfos.ReRead();
        var requestIp = context.Connection.RemoteIpAddress.GetIPV4Address();
        if (!SystemConfig.IpInfos.CurrentConfig.disIps.Contains(requestIp.ToString()))
        {
            var okList = new List<string>() { "/index.html", "/assets", "/api/system", "/api/info", "/api/account" };
            var url = context.Request.Path.ToString().ToLower();
            if (okList.Where(x => url.StartsWith(x)).Any())
            {
                await next();
            }
            else
            {
                await DisResponse(context, requestIp);
            }
        }
        else
        {
            await DisResponse(context, requestIp);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="requestIp"></param>
    /// <returns></returns>
    public async Task DisResponse(HttpContext context, IPAddress requestIp)
    {
        Console.WriteLine($"IP Web Flite :{requestIp} close");
        logger.LogDebug($"【{context.Request.GetDisplayUrl()}】: Close Accept");
        await context.Response.WriteAsync($"<p> {requestIp} Access Denied</p>");
        if (!SystemConfig.IpInfos.CurrentConfig.disIps.Contains(requestIp.ToString()) && !SystemConfig.IpInfos.CurrentConfig.Ips.Contains(requestIp.ToString()))
        {
            SystemConfig.IpInfos.CurrentConfig.disIps.Add(requestIp.ToString());
            SystemConfig.IpInfos.Save();
        }
    }
}
