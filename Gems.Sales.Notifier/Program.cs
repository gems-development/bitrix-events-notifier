using Gems.Sales.Notifier.Bot;
using Gems.Sales.Notifier.Models;
using Gems.Sales.Notifier.UseCases.NotifyTaggedUsers;
using MAX.Bot.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
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
builder.Services.AddMaxBotClient(botToken);
builder.Services.AddScoped<IMessenger, MaxMessenger>();
//Настройка для Consul
builder.Configuration.AddConsul("Gems.Sales.BitrixNotifier/appsettings.json", options => {
        options.ConsulConfigurationOptions = cco => {
            cco.Address = new Uri("http://localhost:8500");
        };
    options.ReloadOnChange = true;
    options.PollWaitTime = TimeSpan.FromSeconds(30);
});

//builder.Services.AddHostedService<BotHostedService>();  !НУЖЕН ТОКЕН ДЛЯ ЗАПУСКА!
var app = builder.Build();

if (app.Environment.IsDevelopment())
	app.MapOpenApi();
app.MapPost("/webhooks", async([FromBody] BitrixWebhookRequestDto request, ISender sender, CancellationToken cancellationToken) =>
	{
        var command = new NotifyTaggedUsersCommand(request.UserIds);
        
        await sender.Send(command, cancellationToken);

        Results.Ok();
    });

await app.RunAsync();