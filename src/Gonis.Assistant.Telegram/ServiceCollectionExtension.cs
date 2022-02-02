using Gonis.Assistant.Core.Bots.Interfaces;
using Gonis.Assistant.Telegram.Handlers;
using Gonis.Assistant.Telegram.Interfaces;
using Gonis.Assistant.Telegram.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Gonis.Assistant.Telegram
{
    /// <summary>
    /// Extension for DI
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Create Telegram bot dependency
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>IServiceCollection with singleton </returns>
        public static IServiceCollection AddTelegramBot(this IServiceCollection services)
        {
            services.AddSingleton<IHandlerService, HandlerService>();
            services.AddSingleton<IBotService, TelegramBotService>();
            return services;
        }
    }
}
