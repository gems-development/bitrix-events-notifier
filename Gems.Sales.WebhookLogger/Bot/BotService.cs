using Gems.Sales.WebhookLogger.Models;
using MAX.Bot.Interfaces;
using MAX.Bot.Interfaces.Models;
using MAX.Bot.Interfaces.Models.Request.Message;
using Microsoft.Extensions.Options;
using Serilog;
using System.Text.RegularExpressions;

namespace Gems.Sales.WebhookLogger.Bot
{
    public class BotService
    {
        private readonly IMaxBotClient _botClient;
        private readonly IOptions<UsersMapOptions> _usersMapOptions;

        public BotService(IMaxBotClient botClient, IOptions<UsersMapOptions> usersMapOptions)
        {
            _botClient = botClient;
            _usersMapOptions = usersMapOptions;
        }

        public async Task StartBot(IOptions<UsersMapOptions> usersMapOptions)
        {
            // Получение обновлений
            _ = _botClient.PollUpdatesWithCallback(
           async (update, client) =>
           {
               if (update is MessageCreatedUpdate messageCreated)
               {
                   var message = messageCreated.Message;
                   Log.Information($"Получено сообщение: {message?.Body?.Text}");
                   string? msgText = message?.Body?.Text;
                   switch(msgText)
                   {
                       case "/start":
                           await SendWelcomeMessage(1234/*вставить айди чата в котором старт написали!*/);
                           break;
                       case "/config":
                           await SendConfigMessage(1234/*вставить айди чата в котором старт написали!*/);
                           break;
                   }
               }
           },
                limit: 100,
                timeout: 90,
                types: new List<string> { UpdateTypes.MessageCreated });
        }
        //Метод для отправки приветственного сообщения
        private async Task SendWelcomeMessage(long chatId)
        {
            await _botClient.SendMessageAsync(new SendMessageRequest
            {
                ChatId = chatId,
                Text = "Добро пожаловать!"
            });
            Log.Information($"Отправлено приветственное сообщение для {chatId}");
        }
        //Метод для отправки конфигурации
        private async Task SendConfigMessage(long chatId)
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

            await _botClient.SendMessageAsync(new SendMessageRequest
            {
                ChatId = chatId,
                Text = configText
            });
            Log.Information($"Отправлено сообщение о конфигурации для {chatId}");
        }
        //не знаю нужен ли он будет
        private static string GetBitrixId(string taggedUser, IOptions<UsersMapOptions> options)
        {
            if (!string.IsNullOrEmpty(taggedUser) && options.Value.Map != null)
            {
                if (options.Value.Map.TryGetValue(taggedUser, out var bitrixId))
                {
                    return bitrixId;
                }
                return string.Empty;
            }
            return string.Empty;
        }
    }
}
