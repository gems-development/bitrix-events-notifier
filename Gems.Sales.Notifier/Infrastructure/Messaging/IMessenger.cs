namespace Gems.Sales.Notifier.Infrastructure.Messaging
{
    public interface IMessenger
    {
        Task SendMessage(long chatId, string message, CancellationToken cancellationToken);
    }
}
