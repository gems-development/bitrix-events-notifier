using Gems.Sales.Notifier.Options;
using MAX.Bot.Interfaces;
using MAX.Bot.Interfaces.Models;
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

        private Task StartBot(CancellationToken stoppingToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var botClient = scope.ServiceProvider.GetRequiredService<IMaxBotClient>();
                var messenger = scope.ServiceProvider.GetRequiredService<IMessenger>();

                _ = botClient.PollUpdatesWithCallback(
                    async (update, _) =>
                    {
                        try
                        {
                            switch (update)
                            {
                                case MessageCreatedUpdate messageCreated:
                                {
                                    await HandleIncomingMessage(messageCreated, messenger, stoppingToken);

                                    break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(
                                ex,
                                "Произошла ошибка при обработке сообщения, отправленного пользователем {UserId}",
                                update is MessageCreatedUpdate messageCreated ? messageCreated.Message?.Sender?.Id : "N/A");
                        }
                    },
                    limit: 100,
                    timeout: 90,
                    types: [UpdateTypes.MessageCreated],
                    cancellationToken: stoppingToken);
            }

            return Task.CompletedTask;
        }

        private async Task<bool> HandleIncomingMessage(
            MessageCreatedUpdate messageCreated,
            IMessenger messenger,
            CancellationToken cancellationToken)
        {
            var message = messageCreated.Message;

            if (message is null)
            {
                Log.Warning("Не удалось извлечь тело сообщения");

                return false;
            }

            var senderId = message.Sender?.Id;

            if (senderId is null)
            {
                Log.Warning("Не удалось определить отправителя сообщения");

                return false;
            }

            var msgText = message.Body?.Text;

            Log.Information("От пользователя {SenderId} получено сообщение: {Text}", senderId, msgText);

            switch (msgText)
            {
                case "/start":
                    await SendWelcomeMessage(messenger, senderId.Value, cancellationToken);
                    break;
                case "/config":
                    await SendConfigMessage(messenger, senderId.Value, cancellationToken);
                    break;
            }

            return true;
        }

        private Task SendWelcomeMessage(IMessenger messenger, long chatId, CancellationToken cancellationToken)
        {
            return messenger.SendMessage(chatId, "Добро пожаловать!", cancellationToken);
        }

        private Task SendConfigMessage(IMessenger messenger, long chatId, CancellationToken cancellationToken)
        {
            var usersMap = _usersMapOptions.Value.Map;
            var configText = "Текущие настройки соответствия идентификаторов \"Битрикс\"-\"Макс\":\n";

            foreach (var kvp in usersMap)
                configText += $"{kvp.Key} = {kvp.Value}\n";

            return messenger.SendMessage(chatId, configText, cancellationToken);
        }
    }
}
