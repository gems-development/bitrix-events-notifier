namespace Gems.Sales.WebhookLogger.Bot
{
    public interface IMessenger
    {
        public Task SendWelcomeMessage(long chatId);
        public Task SendNotification(long chatId);
    }
}
