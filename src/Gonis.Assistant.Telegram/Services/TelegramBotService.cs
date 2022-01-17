using Gonis.Assistant.Core.Bots.Interfaces;
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
        private readonly string _errorChatId;
        private CancellationTokenSource _cts;

        public TelegramBotService(IOptions<TelegramBotOptions> telegramBotOptions)
        {
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

            if (string.IsNullOrWhiteSpace(_telegramBotOptions?.ErrorsChatId))
            {
                throw new ArgumentNullException("Telegram Error Chat Id is empty.");
            }

            _errorChatId = _telegramBotOptions.ErrorsChatId;
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
                    HandleUpdateAsync,
                    HandleErrorAsync,
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
                    IsStarted = false;
                }
            }
            else
            {
                throw new Exception($"{nameof(TelegramBotService)} is already stopped.");
            }
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type != UpdateType.Message)
            {
                return;
            }

            if (update.Message!.Type != MessageType.Text)
            {
                return;
            }

            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;
            var userInfo = string.Join(" ", update.Message.Chat.Id,
                update.Message.Chat.FirstName ?? "FirstName",
                update.Message.Chat.LastName ?? "LastName",
                $"@{update.Message.Chat.Username}{Environment.NewLine}");

            try
            {
                if (!string.IsNullOrWhiteSpace(messageText))
                {
                    if (messageText.Equals("error"))
                    {
                        throw new Exception($"{userInfo}Test error!");
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: $"{messageText}",
                            cancellationToken: cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(botClient, ex, cancellationToken);
            }
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            try
            {
                var errorMessage = exception switch
                {
                    ApiRequestException apiRequestException
                        => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };
                if (IsStarted)
                {
                    botClient.SendTextMessageAsync(
                        chatId: _errorChatId,
                        text: $"{errorMessage}",
                        cancellationToken: cancellationToken).GetAwaiter();
                }
                else
                {
                    throw exception;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return Task.CompletedTask;
        }
    }
}
