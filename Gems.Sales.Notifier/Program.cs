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
builder.Services.AddHttpClient<ISalesManagementSystemClient, BitrixClient>();

//Настройка для Consul
builder.Configuration.AddConsul("Gems.Sales.BitrixNotifier/appsettings.json", options => {
        options.ConsulConfigurationOptions = cco => {
            cco.Address = new Uri("http://localhost:8500");
        };
    options.ReloadOnChange = true;
    options.PollWaitTime = TimeSpan.FromSeconds(30);
});

builder.Services.AddSingleton<IRequestQueue<long>, RequestQueue<long>>();
//builder.Services.AddHostedService<BotHostedService>();  !НУЖЕН ТОКЕН ДЛЯ ЗАПУСКА!
builder.Services.AddHostedService<RequestQueueHandler>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.MapPost("/webhooks", async(
    HttpRequest request, 
    IOptions<SupportedEventsOptions> options, 
    CancellationToken cancellationToken,
    IRequestQueue<long> queue) =>
{
    var form = await request.ReadFormAsync();
    var eventType = form["event"].ToString();

    if (!options.Value.Contains(eventType))
        return Results.Ok();

    // TODO: положить идентификатор комментария в очередь для последующей обработки.
    var commentId = Convert.ToInt64(form["data[FIELDS][ID]"]);
    queue.Enqueue(commentId);

    return Results.Ok();
});
//// ============ ТЕСТОВЫЙ БЛОК ============
//using (var scope = app.Services.CreateScope())
//{
//    var queue = scope.ServiceProvider.GetRequiredService<IRequestQueue<long>>();
//    var client = scope.ServiceProvider.GetRequiredService<ISalesManagementSystemClient>();

//    // Кладём тестовый ID в очередь
//    queue.Enqueue(26890);

//    // Вызываем метод (нужно привести к BitrixClient или добавить метод в интерфейс)
//    if (client is BitrixClient bitrixClient)
//    {
//        await bitrixClient.GetBitrixIds();
//    }
//}
//// =======================================ыы
await app.RunAsync();
//1. ISalesManagmentSystemClient и реализ BitrixClient
//2. DI ISalesManagmentSystemClient BitrixClient
//3. Получить через инъекцию в геттеггедюзерайдис
//4. В БитриксКлиенте получить через диай IHttpClientFactory c помощью которой получить экземпляр http клиента
//5. С помощью хттпклиента сделать запрос к апи битрикса на получение комментария
//6. Вернули ответ и перешли в юзкейс, извлечь айди юзера если есть(может быть много)