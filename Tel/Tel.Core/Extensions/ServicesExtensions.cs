using System.Threading;
using System.Threading.Tasks;
using Tel.Core.Client;
using Tel.Core.Config;
using Tel.Core.Filters;
using Tel.Core.Forwarder;
using Tel.Core.Forwarder.MiddleWare;
using Tel.Core.Handlers.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Yarp.ReverseProxy.Forwarder;

namespace Tel.Core.Extensions;

public static class ServicesExtensions
{

    /// <summary>
    /// 添加服务端后台进程
    /// </summary>
    /// <param name="services"></param>
    public static void AddTelServer(this IServiceCollection services, IConfigurationSection configurationSection)
    {
        services.AddReverseProxy().LoadFromMemory();
        services.AddSingleton<IForwarderHttpClientFactory, TelForwarderHttpClientFactory>();
        services.AddHttpContextAccessor();

        services.Configure<DefaultServerConfig>(configurationSection)
            .AddSingleton<IExceptionFilter, TelExceptionFilter>()
            .AddTransient<ILoginHandler, LoginHandler>()
            .AddSingleton<TelClientHandler>()
            .AddSingleton<TelSwapHandler>()
            .AddSingleton<TelCoreServer>();
            //.AddSingleton<CheckWebAllowAccessIpsHandler>();
    }

    /// <summary>
    /// 服务端中间件
    /// </summary>
    /// <param name="app"></param>
    public static void UseTelServer(this IApplicationBuilder app)
    {
        app.UseWebSockets();

        var swapHandler = app.ApplicationServices.GetRequiredService<TelSwapHandler>();
        var clientHandler = app.ApplicationServices.GetRequiredService<TelClientHandler>();
        //var checkWebAllowAccessIpsHandler = app.ApplicationServices.GetRequiredService<CheckWebAllowAccessIpsHandler>();
        app.Use(clientHandler.Handle);
        app.Use(swapHandler.Handle);
        //app.Use(checkWebAllowAccessIpsHandler.Handle);
    }

    public static void MapTelServer(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapReverseProxy();
        endpoints.MapFallback(context =>
        {
            var options = context.RequestServices.GetRequiredService<IOptionsMonitor<DefaultServerConfig>>();
            var host = context.Request.Host.Host;
            if (!host.EndsWith(options.CurrentValue.WebDomain) || host.Equals(options.CurrentValue.WebDomain))
            {
                context.Response.StatusCode = 404;
                return Task.CompletedTask;
            }

            context.Response.StatusCode = 200;
            context.Response.WriteAsync(TunnelResource.Page_NotFund, CancellationToken.None);
            return Task.CompletedTask;
        });
    }
}
