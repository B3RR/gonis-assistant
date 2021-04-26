using Gonis.Assistant.Core.Constants;
using Gonis.Assistant.Server.HealthCheckers;
using Gonis.Assistant.Telegram.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using System;
using System.Linq;
using System.Net.Http;

namespace Gonis.Assistant.Server.Extensions
{
    /// <summary>
    /// Extension for DI
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Create HttpClient dependency
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>IServiceCollection with singleton </returns>
        public static IServiceCollection AddHttpClientForWebApi(this IServiceCollection services, IConfiguration configuration)
        {
            var webApiEndpoint = configuration
                .GetSection(ConfigurationConstants.MainSection)
                .GetChildren()
                .FirstOrDefault(x => x.Key == ConfigurationConstants.WebApiEndpoint)
                ?.Value;
            if (string.IsNullOrWhiteSpace(webApiEndpoint))
            {
                throw new ArgumentNullException($"Input parameter {ConfigurationConstants.WebApiEndpoint} is empty.");
            }

            var uri = new Uri(webApiEndpoint);
            var httpClient = new HttpClient()
            {
                BaseAddress = uri
            };

            services.AddSingleton(httpClient);
            return services;
        }

        public static IServiceCollection AddLogger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ILogger>(x => new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger());
            return services;
        }

        public static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TelegramBotOptions>(configuration.GetSection(ConfigurationConstants.BotsSection).GetSection(ConfigurationConstants.TelegramBotSection));
            return services;
        }

        public static IHealthChecksBuilder AddCustomHealthChecks(this IServiceCollection services)
        {
            return services
                .AddHealthChecks()
                .AddCheck<OptionsHealthCheck>("options-check", HealthStatus.Degraded, new[] { "options" })
                .AddCheck<MemoryHealthCheck>("memory-check", HealthStatus.Degraded, new[] { "memory" });
        }
    }
}
