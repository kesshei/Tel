using System.Threading;
using System.Threading.Tasks;

namespace Tel.Core.Client
{
    public interface ITelClient 
    {
        Task StartAsync(CancellationToken cancellationToken);

        Task StopAsync(CancellationToken cancellationToken);
    }
}
