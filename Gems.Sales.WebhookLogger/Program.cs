using Gems.Sales.WebhookLogger.Bot;
using Gems.Sales.WebhookLogger.Models;
using MAX.Bot.Extensions;
using MAX.Bot.Interfaces.Models;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
// Настройка Serilog из конфигурации
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

//Привязка конфигурации к классу для user's id
builder.Services.Configure<UsersMapOptions>(builder.Configuration.GetSection(UsersMapOptions.SectionName));

//Проверка наличия токена
var botToken = Environment.GetEnvironmentVariable("BOT_TOKEN") ?? throw new Exception("Токен бота не найден");
Log.Information("Токен бота найден");
builder.Services.AddTransient<BotService>();
builder.Services.AddMaxBotClient(botToken);


var app = builder.Build();

if (app.Environment.IsDevelopment())
	app.MapOpenApi();


app.MapPost("/webhooks", async (HttpRequest request) =>
	{
		request.EnableBuffering();

		using var reader = new StreamReader(request.Body, leaveOpen: true);
		var body = await reader.ReadToEndAsync();

		Log.Information("{Body}", body);
	});
using var scope = app.Services.CreateScope();
var usersMapOptions = scope.ServiceProvider.GetRequiredService<IOptions<UsersMapOptions>>();

/* !НУЖЕН ТОКЕН ДЛЯ ЗАПУСКА!
//Запуск бота
var botService = scope.ServiceProvider.GetRequiredService<BotService>();
await botService.StartBot(usersMapOptions);
*/

/* тест GetBitrixId
string test = BotService.TestGetBitrixId(usersMapOptions);
if (!string.IsNullOrEmpty(test))
{
	Log.Information($"Найден битрикс пользователя {test}");
}
else
{
    Log.Information($"Битрикс пользователя {test} не найден");
} */
await app.RunAsync();