using Gonis.Assistant.Telegram.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Gonis.Assistant.Server.HealthCheckers
{
    public class OptionsHealthCheck : IHealthCheck
    {
        private readonly IHostingEnvironment _env;
        private readonly TelegramBotOptions _telegramBotOptions;

        public OptionsHealthCheck(
            IHostingEnvironment env,
            IOptions<TelegramBotOptions> telegramBotOptions)
        {
            _env = env;
            _telegramBotOptions = telegramBotOptions?.Value;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_telegramBotOptions == null)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("Input params for Telegram Bot is empty."));
            }
            else
            {
                if (string.IsNullOrWhiteSpace(_telegramBotOptions.Name))
                {
                    return Task.FromResult(HealthCheckResult.Unhealthy("Telegram Bot Name is empty."));
                }
                else if (string.IsNullOrWhiteSpace(_telegramBotOptions.Token))
                {
                    return Task.FromResult(HealthCheckResult.Unhealthy("Telegram Bot Token is empty."));
                }
            }

            var telegramBotOptions = JObject.FromObject(_telegramBotOptions);
            var password = telegramBotOptions?.Root["Token"]?.Parent;
            if (password != null)
            {
                password.Remove();
            }

            return Task.FromResult(HealthCheckResult.Healthy(
                description: "Input parameters",
                data: new Dictionary<string, object>
                {
                    {
                        "Environments",
                        new JObject(
                            new JProperty("EnvironmentName", _env.EnvironmentName),
                            new JProperty("IsDevelopment", _env.IsDevelopment()))
                    },
                    {
                        "TelegramBotOptions", JObject.FromObject(telegramBotOptions)
                    }
                }));
        }
    }
}
