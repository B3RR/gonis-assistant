using Gonis.Assistant.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Gonis.Assistant.Core.Middlewares
{
    public class HttpStatusCodeExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public HttpStatusCodeExceptionMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(HttpContext context)
        {
            if (context?.Request?.Path.ToString().StartsWith("/api/", StringComparison.CurrentCultureIgnoreCase) ==
                true)
            {
                try
                {
                    await _next(context);
                    if (context?.Response?.StatusCode == 404)
                    {
                        var url = $"{context?.Request?.Host.Value}{context?.Request?.Path}";
                        if (context?.Response?.HasStarted != true)
                        {
                            // context.Response.Clear();
                            await context.Response.WriteAsync("404 - Endpoint not found - " + url);
                        }
                        else
                        {
                            _logger.Warning($"404 - Response was started - {url}");
                        }
                    }
                }
                catch (HttpStatusCodeException ex)
                {
                    if (context?.Response?.HasStarted == true)
                    {
                        _logger.Warning(ex, $"{ex.StatusCode} - Response was started.");
                        throw;
                    }

                    // context.Response.Clear();
                    context.Response.StatusCode = ex.StatusCode;
                    context.Response.ContentType = ex.ContentType;
                    if (context.Response.StatusCode != 401)
                    {
                        _logger.Error(ex, $"{ex.StatusCode} - {ex.Message}");
                    }

                    await context.Response.WriteAsync($"{ex.StatusCode} - {ex.Message}");
                }
                catch (Exception ex)
                {
                    if (context?.Response?.HasStarted == true)
                    {
                        _logger.Warning(ex, $"500 - Response was started.");
                        throw;
                    }

                    // context.Response.Clear();
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = @"text/plain; charset=utf-8";
                    _logger.Error(ex, $"{ex.GetType().Name} - {ex.Message}");
                    await context.Response.WriteAsync($"500 Server error - {ex.Message}");
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}
