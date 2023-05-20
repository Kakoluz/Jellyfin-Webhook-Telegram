using System.ComponentModel.DataAnnotations;

namespace TelegramWebhooks.Models
{
	public class Chat
	{
		[Key]
		public long Id { get; set; }
		public string? Name { get; set; }
		public string? Description { get; set; }
	}
}
