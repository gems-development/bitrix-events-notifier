namespace Gems.Sales.WebhookLogger.Bot
{
    public interface IMessenger
    {
        Task SendWelcomeMessage(long chatId);
        Task SendNotification(long chatId);
    }
}
