using Gems.Sales.WebhookLogger.Models;
using Microsoft.Extensions.Options;

namespace Gems.Sales.WebhookLogger.Bot
{
    public interface IBitrixService
    {//возможно удалить надо
        string GetBitrixId(string user);
    }
}
