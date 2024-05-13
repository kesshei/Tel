using Tel.Core.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.ExceptionServices;
using System.IO;

namespace Tel.Core.Services
{
    public class ServiceTelClient : IHostedService
    {
        readonly ILogger<ServiceTelClient> _logger;
        readonly ITelClient _fastTunnelClient;

        public ServiceTelClient(ILogger<ServiceTelClient> logger, ITelClient fastTunnelClient)
        {
            _logger = logger;
            _fastTunnelClient = fastTunnelClient;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _fastTunnelClient.StartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _fastTunnelClient.StopAsync(cancellationToken);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                _logger.LogError("【UnhandledException】" + e.ExceptionObject);
                var type = e.ExceptionObject.GetType();
                _logger.LogError("ExceptionObject GetType " + type);
            }
            catch
            {
            }
        }
    }
}
