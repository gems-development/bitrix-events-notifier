using Gems.Sales.WebhookLogger.Bot;
using MAX.Bot.Interfaces;
using MAX.Bot.Interfaces.Models.Request.Message;
using Serilog;

namespace Gems.Sales.WebhookLogger.UseCases.NotifyTaggedUsers
{
    public class MessageSender: IMessenger
    {//Оставить тег метод переназвать в МаксМессенджер и тексты через командхендлер
        private readonly IMaxBotClient _botClient;
        public MessageSender(IMaxBotClient botClient)
        {
            _botClient = botClient;
        }
        //Метод для отправки приветственного сообщения
        public async Task SendWelcomeMessage(long chatId)
        {
            await _botClient.SendMessageAsync(new SendMessageRequest
            {
                ChatId = chatId,
                Text = "Добро пожаловать!"
            });
            Log.Information($"Отправлено приветственное сообщение для {chatId}");
        }
        //Метод для отправки приветственного сообщения
        public async Task SendNotification(long chatId)
        {
            await _botClient.SendMessageAsync(new SendMessageRequest
            {
                ChatId = chatId,
                Text = "Вы упомянуты в лиде"
            });
            Log.Information($"Отправлено уведомление для {chatId}");
        }
    }
}
