using Gonis.Assistant.Core.Bots.Interfaces;
using Gonis.Assistant.Core.Constants;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace Gonis.Assistant.Telegram.Services
{
    public class TelegramBotService : IBotService
    {
        private readonly TelegramBotClient _botClient;
        private readonly IConfiguration _configuration;

        public TelegramBotService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            var token = _configuration
                .GetSection(ConfigurationConstants.BotsSection)
                .GetChildren()
                .FirstOrDefault(x => x.Key == ConfigurationConstants.TelegramBotToken)
                ?.Value;
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException("Telegram Bot token is empty.");
            }

            _botClient = new TelegramBotClient(token);
            _botClient.OnMessage += OnMessageHandler;
        }

        public bool IsStarted { get; private set; }

        public void Start()
        {
            if (!IsStarted)
            {
                _botClient.StartReceiving();
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
                _botClient.StopReceiving();
                IsStarted = false;
            }
            else
            {
                throw new Exception($"{nameof(TelegramBotService)} is already stopped.");
            }
        }

        private async void OnMessageHandler(object? sender, MessageEventArgs e)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            var msg = e.Message;
            if (!string.IsNullOrWhiteSpace(msg.Text))
            {
                var text = string.Join(string.Empty, msg.Text.Reverse());
                await _botClient.SendTextMessageAsync(msg.Chat.Id, text, replyToMessageId: msg.MessageId);
            }
        }
    }
}
