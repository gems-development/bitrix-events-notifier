using Gems.Sales.WebhookLogger.Bot;
using Gems.Sales.WebhookLogger.Handlers;
using Gems.Sales.WebhookLogger.Models;
using Gems.Sales.WebhookLogger.UseCases.NotifyTaggedUsers;
using MAX.Bot.Extensions;
using MAX.Bot.Interfaces.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using Winton.Extensions.Configuration.Consul;

var builder = WebApplication.CreateBuilder(args);

//Настройка MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

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

builder.Services.AddSingleton<IBitrixService, BitrixService>();
builder.Services.AddSingleton<IMessenger, MessageSender>();
builder.Services.AddSingleton<IMessageHandler, MessageHandler>();
builder.Services.AddSingleton<BotService>();
//Настройка для Consul
builder.Configuration.AddConsul("Gems.Sales.BitrixNotifier/appsettings.json", options => {
        options.ConsulConfigurationOptions = cco => {
            cco.Address = new Uri("http://localhost:8500");
        };
    options.ReloadOnChange = true;
    options.PollWaitTime = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
	app.MapOpenApi();
app.MapPost("/webhooks", async([FromBody] BitrixWebhookRequestDto request, ISender sender, CancellationToken cancellationToken) =>
	{
        var command = new NotifyTaggedUsersCommand(request.UserIds);
        
        await sender.Send(command, cancellationToken);

        Results.Ok();
    });
using var scope = app.Services.CreateScope();
var usersMapOptions = scope.ServiceProvider.GetRequiredService<IOptions<UsersMapOptions>>();

/* !НУЖЕН ТОКЕН ДЛЯ ЗАПУСКА!
//Запуск бота
var botService = scope.ServiceProvider.GetRequiredService<BotService>();
await botService.StartBot(usersMapOptions);
*/
// тест GetBitrixId
//TestBitrix();
await app.RunAsync();
/*
void TestBitrix()
{
    string test = BotService.TestGetBitrixId(usersMapOptions);
    if (!string.IsNullOrEmpty(test))
    {
        Log.Information($"Найден битрикс пользователя {test}");
    }
    else
    {
        Log.Information($"Битрикс пользователя {test} не найден");
    }
}*/