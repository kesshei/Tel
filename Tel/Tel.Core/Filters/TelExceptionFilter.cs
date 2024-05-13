using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tel.Core.Extensions;

namespace Tel.Core.Filters
{
    public class TelExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<TelExceptionFilter> logger;

        public TelExceptionFilter(
            ILogger<TelExceptionFilter> logger,
            IWebHostEnvironment hostingEnvironment)
        {
            this.logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        public void OnException(ExceptionContext context)
        {
            if (!_hostingEnvironment.IsDevelopment())
            {
                return;
            }

            logger.LogError(context.Exception, "[全局异常]");
        }
    }
}
