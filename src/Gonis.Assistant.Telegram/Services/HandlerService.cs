using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Gonis.Assistant.Core.Bots.Interfaces;
using Gonis.Assistant.Telegram.Interfaces;
using Gonis.Assistant.Telegram.Options;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Gonis.Assistant.Telegram.Handlers
{
    public class HandlerService : IHandlerService
    {
        private readonly TelegramBotOptions _telegramBotOptions;
        private readonly long _errorChatId;

        public HandlerService(IOptions<TelegramBotOptions> telegramBotOptions)
        {
            _telegramBotOptions = telegramBotOptions?.Value ?? throw new ArgumentNullException(nameof(telegramBotOptions));
            if (string.IsNullOrWhiteSpace(_telegramBotOptions?.ErrorsChatId))
            {
                throw new ArgumentNullException("Telegram Error Chat Id is empty.");
            }
            if (long.TryParse(_telegramBotOptions.ErrorsChatId, out var errorChatId) && errorChatId > 0)
            {
                _errorChatId = errorChatId;
            }
        }

        public Task ErrorHandlerAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {

            if (!cancellationToken.IsCancellationRequested)
            {
                var errorMessage = exception switch
                {
                    ApiRequestException apiRequestException
                        => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };
                return SendMessageAsync(botClient,
                                chatId: _errorChatId,
                                messageText: errorMessage,
                                cancellationToken: cancellationToken);
            }
            else
            {
                throw exception;
            }
        }


        public async Task SendMessageAsync(ITelegramBotClient botClient, long chatId, string messageText, CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(
                         chatId: chatId,
                         text: $"{messageText}",
                         cancellationToken: cancellationToken);
        }

        public Task UpdateHandlerAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type != UpdateType.Message)
            {
                return Task.CompletedTask;
            }

            if (update.Message!.Type != MessageType.Text)
            {
                return Task.CompletedTask;
            }

            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;
            
            try
            {
                if (!string.IsNullOrWhiteSpace(messageText))
                {
                    if (messageText.Equals("error"))
                    {
                        var messageParams = new List<string>();
                        messageParams.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        messageParams.Add(chatId.ToString());
                        if (!string.IsNullOrWhiteSpace(update.Message.Chat.FirstName))
                        {
                            messageParams.Add($"Firstname: {update.Message.Chat.FirstName}");
                        }

                        if (!string.IsNullOrWhiteSpace(update.Message.Chat.LastName))
                        {
                            messageParams.Add($"Lastname: {update.Message.Chat.LastName}");
                        }
                        messageParams.Add($"Username: @{update.Message.Chat.Username}");
                        messageParams.Add($"Message: {messageText}");
                        messageParams.Add(Environment.NewLine);
                        var userInfo = string.Join(Environment.NewLine, messageParams);
                        throw new Exception($"{userInfo} Test error!");
                    }
                    else
                    {
                        return SendMessageAsync(botClient, chatId, messageText, cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                return ErrorHandlerAsync(botClient, ex, cancellationToken);
            }
            return Task.CompletedTask;
        }
    }
}

