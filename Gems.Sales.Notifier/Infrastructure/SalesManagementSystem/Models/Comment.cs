namespace Gems.Sales.Notifier.Infrastructure.SalesManagementSystem.Models
{
    public class Comment
    {
        public long Id { get; set; }
        public long DealId { get; set; }
        public string? Body { get; set; }
    }
}
