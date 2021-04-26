using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Gonis.Assistant.Core.Middlewares
{
    public class OptionsVerbMiddleware
    {
        private readonly RequestDelegate _next;

        public OptionsVerbMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext context)
        {
            if ((string)context.Request.Headers["Origin"] != null && context.Request.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 200;
                context.Response.Headers["Access-Control-Allow-Credentials"] = "true";
                context.Response.Headers["Access-Control-Allow-Headers"] = context.Request.Headers["Access-Control-Request-Headers"];
                context.Response.Headers["Access-Control-Allow-Methods"] = context.Request.Headers["Access-Control-Request-Method"];
                context.Response.Headers["Access-Control-Allow-Origin"] = context.Request.Headers["Origin"];
                await context.Response.WriteAsync("Allowed");
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }
}
