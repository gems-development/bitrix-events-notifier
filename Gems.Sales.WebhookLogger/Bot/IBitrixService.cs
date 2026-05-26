using Gems.Sales.WebhookLogger.Models;
using Microsoft.Extensions.Options;

namespace Gems.Sales.WebhookLogger.Bot
{
    public interface IBitrixService
    {
        string GetBitrixId(string user);
    }
}
