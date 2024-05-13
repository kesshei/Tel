using Tel.Core.Config;
using Tel.Core.Handlers.Client;
using Tel.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tel.Core.Client.Extensions
{
    public static class ServicesExtensions
    {
        /// <summary>
        /// 客户端依赖及HostedService
        /// </summary>
        /// <param name="services"></param>
        public static void AddTelClient(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            services.Configure<DefaultClientConfig>(configurationSection);

            services.AddTransient<ITelClient, TelClient>()
                .AddSingleton<LogHandler>()
                .AddSingleton<SwapHandler>();

            services.AddHostedService<ServiceTelClient>();
        }

    }
}
