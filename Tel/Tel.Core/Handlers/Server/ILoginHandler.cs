using System.Threading;
using System.Threading.Tasks;
using Tel.Core.Client;
using Tel.Core.Models;

namespace Tel.Core.Handlers.Server;

public interface ILoginHandler
{
    Task<bool> HandlerMsg(TelCoreServer TelServer, TunnelClient tunnelClient, string lineCmd, CancellationToken cancellationToken);
}
