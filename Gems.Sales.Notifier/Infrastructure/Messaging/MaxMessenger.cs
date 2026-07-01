using MAX.Bot.Interfaces;
using MAX.Bot.Interfaces.Models.Request.Message;

namespace Gems.Sales.Notifier.Infrastructure.Messaging
{
    internal sealed class MaxMessenger : IMessenger
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
