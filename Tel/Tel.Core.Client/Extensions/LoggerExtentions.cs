using Microsoft.Extensions.Logging;
using System;

namespace Tel.Core.Extensions
{
    public static class LoggerExtentions
    {
        public static void LogError(this ILogger logger, Exception ex)
        {
            logger.LogError(ex, string.Empty);
        }
    }
}
