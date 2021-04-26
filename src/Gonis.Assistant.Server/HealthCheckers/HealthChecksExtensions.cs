using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace Gonis.Assistant.Server.HealthCheckers
{
    public static class HealthChecksExtensions
    {
        /// <summary>
        /// Add custom UI and JSON endpoints to pipeline
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        /// <returns>IApplicationBuilder instance</returns>
        public static IApplicationBuilder UseCustomHealthCheckEndpoints(this IApplicationBuilder app)
        {
            app
                .UseHealthChecks(
                    "/health/detail",
                    new HealthCheckOptions
                    {
                        Predicate = _ => true,
                        AllowCachingResponses = false,
                        ResponseWriter = WriteResponse
                    })
                .UseHealthChecks("/health/ready", new HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains("ready"),
                    AllowCachingResponses = false
                })
                .UseHealthChecks("/health/live", new HealthCheckOptions
                {
                    // Exclude all checks and return a 200-Ok
                    Predicate = _ => false,
                    AllowCachingResponses = false
                });

            return app;
        }

        private static Task WriteResponse(
            HttpContext httpContext,
            HealthReport result)
        {
            httpContext.Response.ContentType = "application/json";

            var json = new JObject(
                new JProperty("status", result.Status.ToString()),
                new JProperty("results", new JObject(result.Entries.Select(pair =>
                    new JProperty(pair.Key, new JObject(
                        new JProperty("status", pair.Value.Status.ToString()),
                        new JProperty("description", pair.Value.Description),
                        new JProperty("data", new JObject(pair.Value.Data.Select(
                            p => new JProperty(p.Key, p.Value))))))))));
            return httpContext.Response.WriteAsync(
                json.ToString(Formatting.Indented));
        }
    }
}
