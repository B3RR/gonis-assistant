using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gonis.Assistant.Core.Middlewares
{
    public class LoggerWebApiRequestMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger logger;

        public LoggerWebApiRequestMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext?.Request?.Path.ToString().StartsWith("/api/", StringComparison.CurrentCultureIgnoreCase) == true)
            {
                logger.Debug($"Web Api Request [{httpContext.Request.Path}]");
                using (LogContext.PushProperty("UserName", httpContext.User?.Identity?.Name))
                using (LogContext.PushProperty("InnerRequestId", Guid.NewGuid().ToString()))
                using (LogContext.PushProperty("Method", httpContext.Request.Method))
                {
                    await _next.Invoke(httpContext);
                }
            }
            else
            {
                await _next.Invoke(httpContext);
            }
        }
    }
}
