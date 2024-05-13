using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tel.Core.Extensions
{
    public static class TaskCompletionSourceExtensions
    {
        public static void SetTimeOut<T>(this TaskCompletionSource<T> tcs, int timeoutMs, Action? action)
        {
            var ct = new CancellationTokenSource(timeoutMs);
            ct.Token.Register(() =>
            {
                if (tcs.Task.IsCompleted)
                    return;

                tcs.TrySetCanceled();
                action?.Invoke();
            }, useSynchronizationContext: false);
        }
    }
}
