namespace Gems.Sales.Notifier.Infrastructure.SalesManagementSystem
{
    public interface ISalesManagementSystemClient
    {
        public Task<string?> GetComment(long commentId);
    }
}
