using Gems.Sales.WebhookLogger.Models;
using MAX.Bot.Interfaces;
using MAX.Bot.Interfaces.Models;
using MAX.Bot.Interfaces.Models.Request.Message;
using Microsoft.Extensions.Options;
using Serilog;
using System.Text.RegularExpressions;
using Gems.Sales.WebhookLogger.Handlers;

namespace Gems.Sales.WebhookLogger.Bot
{
    public class BotService
    {
        private readonly IMaxBotClient _botClient;
        private readonly IMessageHandler _messageHandler;

        public BotService(IMaxBotClient botClient, IMessageHandler messageHandler)
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
                   var message = messageCreated.Message;
                   Log.Information($"Получено сообщение: {message?.Body?.Text}");
                   string? msgText = message.Body?.Text;
                   await _messageHandler.HandleAsync(msgText);
               }
           },
                limit: 100,
                timeout: 90,
                types: new List<string> { UpdateTypes.MessageCreated });
        }
    }
}
