using Gems.Sales.Notifier.Infrastructure.SalesManagementSystem.Models;
namespace Gems.Sales.Notifier.Infrastructure.SalesManagementSystem
{
    public interface ISalesManagementSystemClient
    {
        public Task<Comment?> GetComment(long id, CancellationToken cancellationToken);
        public Task<Deal?> GetDeal (long id, CancellationToken cancellationToken);
    }
}
