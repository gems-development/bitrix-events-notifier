using Gems.Sales.WebhookLogger.Bot;
using MAX.Bot.Interfaces;
using MAX.Bot.Interfaces.Models.Request.Message;
using Serilog;

namespace Gems.Sales.WebhookLogger.UseCases.NotifyTaggedUsers
{
    public class MaxMessenger: IMessenger
    {
        private readonly IMaxBotClient _botClient;
        public MaxMessenger(IMaxBotClient botClient)
        {
            _botClient = botClient;
        }
        
        //Метод для отправки уведомления о сделке
        public Task SendNotification(long chatId, string text)
        {
            return _botClient.SendMessageAsync(new SendMessageRequest
            {
                ChatId = chatId,
                Text = text
            });
        }
    }
}
