using Microsoft.Extensions.Options;

namespace Gems.Sales.WebhookLogger.Models
{
    public class UsersMapOptions
    {

        public const string SectionName = "UsersMap";
        public Dictionary<string, string> Map { get; set; } = new();
    }
}
