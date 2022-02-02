using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Gonis.Assistant.Telegram.Interfaces
{
	public interface IHandlerService
	{
		Task ErrorHandlerAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken);
		Task UpdateHandlerAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
		Task SendMessageAsync(ITelegramBotClient botClient,
			long chatId,
			string messageText,
			CancellationToken cancellationToken);
	}
}

