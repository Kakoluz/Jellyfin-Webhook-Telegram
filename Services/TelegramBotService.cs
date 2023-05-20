using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramWebhooks.Helpers;
using TelegramWebhooks.Models;

namespace TelegramWebhooks.Services
{
	public interface ITelegramBotService
	{
		public void sendNotificationAdmin(string update);
		public void sendNotification(Notification update);
	}
	
	public class TelegramBotService : ITelegramBotService
	{
		private readonly string telegramBotToken;

		private readonly TelegramBotClient botClient;

		private IServiceScopeFactory scopeFactory;

		private readonly CancellationTokenSource cts = new CancellationTokenSource();

		private readonly long AdminId;

		public TelegramBotService(IOptions<AppSettings> appSetting, IServiceScopeFactory scopeFactory)
		{
			this.scopeFactory = scopeFactory;
			AdminId = appSetting.Value.AdminId;
			telegramBotToken = appSetting.Value.Token != null ? appSetting.Value.Token : throw new ArgumentNullException(nameof(appSetting.Value.Token));
			botClient = new TelegramBotClient(telegramBotToken);

			botClient.StartReceiving(
				updateHandler: HandleUpdateAsync,
				pollingErrorHandler: HandlePollingErrorAsync,
				receiverOptions: new() { AllowedUpdates = Array.Empty<UpdateType>() },
				cancellationToken: cts.Token
			);
		}

		public void sendNotificationAdmin(string update)
		{
			sendMessage(AdminId, update);
		}
			
		public void sendNotification(Notification update)
		{
			using (var scope = scopeFactory.CreateScope())
			{
				var dataBase = scope.ServiceProvider.GetRequiredService<TelegramChatsContext>();

				foreach (var chat in dataBase.Chats)
				{
					if (!sendPhotoMessage(chat.Id, update.photo, update.message))
					{
						if (!sendMessage(chat.Id, update.message))
						{
							dataBase.Chats.Remove(chat);
							dataBase.SaveChanges();
						}
					}
				}
			}
		}

		private bool sendMessage(long Id, string content)
		{
			try
			{
				botClient.SendTextMessageAsync(
					chatId: Id,
					text: content,
					parseMode: ParseMode.Html,
					cancellationToken: cts.Token
				);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private bool sendPhotoMessage(long Id, string image, string content)
		{
			try
			{
				botClient.SendPhotoAsync(
					chatId: Id,
					photo: InputFile.FromUri(image),
					caption: content,
					parseMode: ParseMode.Html,
					cancellationToken: cts.Token
				);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
		{
			if (update.Message is not { } message)
				return Task.CompletedTask;

			using (var scope = scopeFactory.CreateScope())
			{
				var dataBase = scope.ServiceProvider.GetRequiredService<TelegramChatsContext>();
				if (message.Text == "/join" || message.Text == "/start")
				{
					if (dataBase.Chats.Find(message.Chat.Id) == null)
					{
						dataBase.Chats.Add(new Models.Chat { Id = message.Chat.Id, Name = message.Chat.Username, Description = message.Chat.Description });
						dataBase.SaveChanges();
						sendMessage(message.Chat.Id, "Chat registered in notification list.");
					}
					else
					{
						sendMessage(message.Chat.Id, "Chat already registered.");
					}
				}
				if (message.Text == "/leave")
				{
					if (dataBase.Chats.Find(message.Chat.Id) != null)
					{
						dataBase.Chats.Remove(dataBase.Chats.Find(message.Chat.Id));
						dataBase.SaveChanges();
						sendMessage(message.Chat.Id, "Notifications disabled on this chat.");
					}
					else
					{
						sendMessage(message.Chat.Id, "Chat not registered.");
					}
				}
			}
			return Task.CompletedTask;
		}

		private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
		{
			var ErrorMessage = exception switch
			{
				ApiRequestException apiRequestException
					=> $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
				_ => exception.ToString()
			};

			Console.WriteLine(ErrorMessage);
			return Task.CompletedTask;
		}
	}
}
