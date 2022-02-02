using Gonis.Assistant.Core.Bots.Interfaces;
using Gonis.Assistant.Telegram.Interfaces;
using Gonis.Assistant.Telegram.Options;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Gonis.Assistant.Telegram.Services
{
    public class TelegramBotService : IBotService
    {
        private readonly TelegramBotClient _botClient;
        private readonly TelegramBotOptions _telegramBotOptions;
        private readonly IHandlerService _handlerService;
        private CancellationTokenSource _cts;

        public TelegramBotService(IHandlerService handlerService, IOptions<TelegramBotOptions> telegramBotOptions)
        {
            _handlerService = handlerService ?? throw new ArgumentNullException(nameof(handlerService));
            _telegramBotOptions = telegramBotOptions?.Value ?? throw new ArgumentNullException(nameof(telegramBotOptions));
            var token = _telegramBotOptions.Token;
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException("Telegram Bot token is empty.");
            }

            var botName = _telegramBotOptions.Name;

            if (string.IsNullOrWhiteSpace(botName))
            {
                throw new ArgumentNullException("Telegram Bot Name is empty.");
            }

            Name = botName;

            _botClient = new TelegramBotClient(token);
        }

        public bool IsStarted { get; private set; }

        public string Name { get; private set; }

        public void Start()
        {
            if (!IsStarted)
            {
                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = { }
                };
                _cts = new CancellationTokenSource();
                _botClient.StartReceiving(
                    _handlerService.UpdateHandlerAsync,
                    _handlerService.ErrorHandlerAsync,
                    receiverOptions,
                    cancellationToken: _cts.Token);
                IsStarted = true;
            }
            else
            {
                throw new Exception($"{nameof(TelegramBotService)} is already started.");
            }
        }

        public void Stop()
        {
            if (IsStarted)
            {
                if (_cts != null)
                {
                    _cts.Cancel();
                    _cts.Dispose();
                }
                IsStarted = false;
            }
            else
            {
                throw new Exception($"{nameof(TelegramBotService)} is already stopped.");
            }
        }



       
    }
}
