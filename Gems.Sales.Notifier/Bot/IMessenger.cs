namespace Gems.Sales.Notifier.Bot
{
    public interface IMessenger
    {
        Task SendNotification(long chatId, string text);
    }
}
