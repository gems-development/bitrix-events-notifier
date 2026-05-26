namespace Gems.Sales.WebhookLogger.Handlers
{
    public interface IMessageHandler
    {
        Task HandleAsync(string? messageText);
    }
}
