using Gems.Sales.Notifier.Options;
using MAX.Bot.Interfaces;
using MAX.Bot.Interfaces.Models;
using MAX.Bot.Interfaces.Models.Request.Message;
using Microsoft.Extensions.Options;
using Serilog;

namespace Gems.Sales.Notifier.Infrastructure.Messaging
{
    internal sealed class BotHostedService : IHostedService
    {
        private readonly IOptions<UsersMapOptions> _usersMapOptions;
        private readonly IServiceScopeFactory _scopeFactory;

        public BotHostedService(IOptions<UsersMapOptions> usersMapOptions, IServiceScopeFactory scopeFactory)
        {
            _usersMapOptions = usersMapOptions;
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken stoppingToken)
        {
            Log.Information("Бот запущен");
            await StartBot(stoppingToken);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            Log.Information("Бот остановлен");

            return Task.CompletedTask;
        }

        public async Task StartBot(CancellationToken stoppingToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var botClient = scope.ServiceProvider.GetRequiredService<IMaxBotClient>();
                // Получение обновлений
                _ = botClient.PollUpdatesWithCallback(
               async (update, client) =>
               {
                   if (update is MessageCreatedUpdate messageCreated)
                   {
                       var message = messageCreated.Message;
                       Log.Information($"Получено сообщение: {message?.Body?.Text}");
                       string? msgText = message?.Body?.Text;
                       using (var innerScope = _scopeFactory.CreateScope())
                       {
                           var innerBotClient = innerScope.ServiceProvider.GetRequiredService<IMaxBotClient>();
                           switch (msgText)
                           {
                               case "/start":
                                   await SendWelcomeMessage(innerBotClient, 1234/*вставить айди чата в котором старт написали!*/);
                                   break;
                               case "/config":
                                   await SendConfigMessage(innerBotClient, 1234/*вставить айди чата в котором старт написали!*/);
                                   break;
                           }
                       }
                   }
               },
                    limit: 100,
                    timeout: 90,
                    types: new List<string> { UpdateTypes.MessageCreated });
            }
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        //Метод для отправки приветственного сообщения
        private async Task SendWelcomeMessage(IMaxBotClient botClient, long chatId)
        {
            await botClient.SendMessageAsync(new SendMessageRequest
            {
                ChatId = chatId,
                Text = "Добро пожаловать!"
            });
            Log.Information($"Отправлено приветственное сообщение для {chatId}");
        }

        //Метод для отправки конфигурации
        private async Task SendConfigMessage(IMaxBotClient botClient, long chatId)
        {
            var usersMap = _usersMapOptions.Value.Map;
            var configText = "Конфигурация UsersMap:\n";
            if (usersMap != null)
            {
                foreach (var kvp in usersMap)
                    configText += $"{kvp.Key} = {kvp.Value}\n";
            }
            else
            {
                configText += "Нет данных.";
            }

            await botClient.SendMessageAsync(new SendMessageRequest
            {
                ChatId = chatId,
                Text = configText
            });
            Log.Information($"Отправлено сообщение о конфигурации для {chatId}");
        }
    }
}
