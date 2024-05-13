using System.Threading;
using System.Threading.Tasks;
using Tel.Core.Client;
using Microsoft.Extensions.Logging;

namespace Tel.Core.Handlers.Client;

public class LogHandler : IClientHandler
{
    private readonly ILogger<LogHandler> _logger;

    public LogHandler(ILogger<LogHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandlerMsgAsync(TelClient cleint, string msg, CancellationToken cancellationToken)
    {
        _logger.LogInformation(msg);
        await Task.CompletedTask;
    }
}
