using Microsoft.EntityFrameworkCore;

namespace TelegramWebhooks.Models
{
	public class TelegramChatsContext : DbContext
	{
		public TelegramChatsContext()
		{

		}
		public TelegramChatsContext(DbContextOptions<TelegramChatsContext> options) : base(options)
		{

		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			
		}

		public DbSet<Chat> Chats { get; set; } = null!;
	}
}
