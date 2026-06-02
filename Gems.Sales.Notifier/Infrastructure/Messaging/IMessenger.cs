namespace Gems.Sales.Notifier.Infrastructure.Messaging
{
    public interface IMessenger
    {
        Task SendNotification(long chatId, string text);
    }
}
