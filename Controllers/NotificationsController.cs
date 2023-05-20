using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TelegramWebhooks.Models;
using TelegramWebhooks.Services;

namespace TelegramWebhooks.Controllers
{
	[ApiController]
	[Route("notifications")]
	public class NotificationsController : Controller
	{

		private readonly ITelegramBotService telegramBotService;

		public NotificationsController(ITelegramBotService telegramBotService)
		{
			this.telegramBotService = telegramBotService;
		}

		[HttpGet]
		[Route("healthcheck")]
		public ActionResult<string> General()
		{
			return Ok("Service working as intented");
		}

		// POST: Notifications/General
		[HttpPost]
		[Route("general")]
		public ActionResult General([FromBody] string message)
		{
			try
			{
				Notification? update = JsonConvert.DeserializeObject<Notification>(message);
				Console.WriteLine("Recieved message:\n" + message);
				if (update == null || (update.message == null && update.photo == null))
				{
					Console.Error.WriteLine("Error in message format");
					return BadRequest();
				}
				telegramBotService.sendNotification(update);
			}
			catch
			{
				return BadRequest();
			}
			return Ok();
		}

		// POST: Notifications/Admin
		[HttpPost]
		[Route("admin")]
		public ActionResult Admin(string message)
		{
			try
			{
				telegramBotService.sendNotificationAdmin(message);
			}
			catch
			{
				return BadRequest();
			}
			return Ok();
		}
	}
}
