using Gems.Sales.WebhookLogger.Bot;
using Gems.Sales.WebhookLogger.Models;
using MAX.Bot.Extensions;
using MAX.Bot.Interfaces.Models;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
//Привязка конфигурации к классу для user's id
builder.Services.Configure<UsersMapOptions>(builder.Configuration.GetSection(UsersMapOptions.SectionName));

//Проверка наличия токена
var botToken = Environment.GetEnvironmentVariable("BOT_TOKEN") ?? throw new Exception("Токен бота не найден");
builder.Services.AddTransient<BotService>();
builder.Services.AddMaxBotClient(botToken);


var app = builder.Build();

if (app.Environment.IsDevelopment())
	app.MapOpenApi();

Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.WriteTo.File("/var/log/gems/bitrix-notifier/webhooks.log")
	.CreateLogger();

app.MapPost("/webhooks", async (HttpRequest request) =>
	{
		request.EnableBuffering();

		using var reader = new StreamReader(request.Body, leaveOpen: true);
		var body = await reader.ReadToEndAsync();

		Log.Logger.Information("{Body}", body);
	});
/* !ПРОВЕРЯЕТ ЕСТЬ ЛИ ПОЛЬЗОВАТЕЛЬ В КОНФИГЕ!(для себя, можно удалить)
using var scope = app.Services.CreateScope();
var usersMapOptions = scope.ServiceProvider.GetRequiredService<IOptions<UsersMapOptions>>();
UsersMapOptions.Test(usersMapOptions);
*/
/* !НУЖЕН ТОКЕН ДЛЯ ЗАПУСКА!
//Запуск бота
var botService = scope.ServiceProvider.GetRequiredService<BotService>();
await botService.StartBot(usersMapOptions);
*/
await app.RunAsync();