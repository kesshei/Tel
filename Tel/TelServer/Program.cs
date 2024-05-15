using System.Net.Http;
using System.Net.Sockets;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace TelServer;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console().WriteTo.File("Logs/log-.log", rollingInterval: RollingInterval.Day)
            .CreateBootstrapLogger();

        try
        {
            CreateHostBuilder(args).Build().Run();

        }
        catch (System.Exception ex)
        {
            Log.Fatal(ex, "致命异常");
            throw;
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .WriteTo.Console())
            .UseWindowsService()
            .ConfigureWebHost(webHostBuilder =>
            {
                // Use TelHostingStartup
                webHostBuilder.UseSetting(WebHostDefaults.HostingStartupAssembliesKey, "Tel.Api");

                webHostBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config.AddJsonFile("config/appsettings.json", optional: false, reloadOnChange: true);
                });
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}
