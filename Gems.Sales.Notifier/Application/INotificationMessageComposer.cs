namespace Gems.Sales.Notifier.Application
{
    public interface INotificationMessageComposer
    {
        public string BuildMessage(string? title);
    }
}
