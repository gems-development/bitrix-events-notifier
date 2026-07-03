using Gems.Sales.Notifier.Application;
using Gems.Sales.Notifier.Infrastructure;
using Gems.Sales.Notifier.Infrastructure.Messaging;
using Gems.Sales.Notifier.Infrastructure.SalesManagementSystem;
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
builder.Services.Configure<BitrixOptions>(builder.Configuration.GetSection(BitrixOptions.SectionName));

//Проверка наличия токена
var botToken = Environment.GetEnvironmentVariable("BOT_TOKEN") ?? throw new Exception("Токен бота не найден");
Log.Information("Токен бота найден");
builder.Services.AddMaxBotClient(botToken);
builder.Services.AddScoped<IMessenger, MaxMessenger>();
builder.Services.AddScoped<ISalesManagementSystemClient, BitrixClient>();
builder.Services.AddScoped<INotificationMessageComposer, NotificationMessageComposer>();
builder.Services.AddHttpClient<ISalesManagementSystemClient, BitrixClient>();

//Настройка для Consul
builder.Configuration.AddConsul("Gems.Sales.BitrixNotifier/appsettings.json", options => {
        options.ConsulConfigurationOptions = cco => {
            cco.Address = new Uri("http://localhost:8500");
        };
    options.ReloadOnChange = true;
    options.PollWaitTime = TimeSpan.FromSeconds(30);
});
builder.Services.AddScoped<IUserIdExtractor, UserIdExtractor>();
builder.Services.AddSingleton<IRequestQueue<long>, RequestQueue<long>>();
//builder.Services.AddHostedService<BotHostedService>(); // !НУЖЕН ТОКЕН ДЛЯ ЗАПУСКА!
builder.Services.AddHostedService<RequestQueueHandler>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.MapPost("/webhooks", async(
    HttpRequest request,
    IOptions<SupportedEventsOptions> options,
    IRequestQueue<long> queue) =>
{
    var form = await request.ReadFormAsync();
    var eventType = form["event"].ToString();

    if (!options.Value.Contains(eventType))
        return Results.Ok();

    var commentId = Convert.ToInt64(form["data[FIELDS][ID]"]);
    queue.Enqueue(commentId);

    return Results.Ok();
});

await app.RunAsync();