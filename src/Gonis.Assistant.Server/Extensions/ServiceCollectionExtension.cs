using Gonis.Assistant.Core.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
    }
}
