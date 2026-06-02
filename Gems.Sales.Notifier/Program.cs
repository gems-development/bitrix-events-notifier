using Gems.Sales.Notifier.Infrastructure.Messaging;
using Gems.Sales.Notifier.Options;
using MAX.Bot.Extensions;
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
builder.Services.Configure<SupportedEventsOptions>(builder.Configuration.GetSection(SupportedEventsOptions.SectionName));
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

app.MapPost("/webhooks", async(HttpRequest request, IOptions<SupportedEventsOptions> options, CancellationToken cancellationToken) =>
{
    var form = await request.ReadFormAsync();
    var eventType = form["userevent"].ToString();

    if (!options.Value.Contains(eventType))
        return Results.Ok();

    // TODO: положить идентификатор комментария в очередь для последующей обработки.

    return Results.Ok();
});

await app.RunAsync();
