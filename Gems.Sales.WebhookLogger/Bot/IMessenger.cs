namespace Gems.Sales.WebhookLogger.Bot
{
    public interface IMessenger
    {
        Task SendNotification(long chatId, string text);
    }
}
