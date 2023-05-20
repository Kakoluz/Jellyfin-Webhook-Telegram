using Microsoft.EntityFrameworkCore;
using TelegramWebhooks.Helpers;
using TelegramWebhooks.Models;
using TelegramWebhooks.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddMvc(options =>
{
	options.InputFormatters.Insert(0, new TextPlainInputFormatter());
	options.OutputFormatters.Insert(0, new TextPlainOutputFormatter());
});

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

var optionsBuilder = new DbContextOptionsBuilder<TelegramChatsContext>();
optionsBuilder.UseSqlite(builder.Configuration.GetConnectionString("MyConnectionString"));

var dbContext = new TelegramChatsContext(optionsBuilder.Options);
dbContext.Database.EnsureCreated();
dbContext.Database.Migrate();

builder.Services.AddDbContext<TelegramChatsContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("MyConnectionString")));
builder.Services.AddSingleton<ITelegramBotService, TelegramBotService>();

var app = builder.Build();

app.Services.GetService<ITelegramBotService>().sendNotificationAdmin("Notification service started");

// Configure the HTTP request pipeline.

app.MapControllers();

app.Run();
