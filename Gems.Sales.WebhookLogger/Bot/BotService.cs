using Gems.Sales.WebhookLogger.Models;
using MAX.Bot.Interfaces;
using MAX.Bot.Interfaces.Models;
using MAX.Bot.Interfaces.Models.Request.Message;
using Microsoft.Extensions.Options;

namespace Gems.Sales.WebhookLogger.Bot
{
    public class BotService
    {
        private readonly IMaxBotClient _botClient;

        public BotService(IMaxBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task StartBot(IOptions<UsersMapOptions> usersMapOptions)
        {
            // Получение обновлений
            _ = _botClient.PollUpdatesWithCallback(
                async (update, client) =>
                {
                    if (update is MessageCreatedUpdate messageCreated)
                    {
                        Console.WriteLine($"Сообщение: {messageCreated.Message?.Body?.Text}");

                        if (messageCreated.Message?.Body?.Text == "/start")
                        {
                            await SendWelcomeMessage(Convert.ToInt32(usersMapOptions));
                        }
                    }
                },
                limit: 100,
                timeout: 90,
                types: new List<string> { UpdateTypes.MessageCreated });
        }
        //Метод для отправки сообщения
        public async Task SendWelcomeMessage(long chatId)
        {
            Console.WriteLine($"Бот отправил сообщение {chatId}");
            await _botClient.SendMessageAsync(new SendMessageRequest
            {
                ChatId = chatId,
                Text = "Добро пожаловать!"
            });
        }
    }
}
