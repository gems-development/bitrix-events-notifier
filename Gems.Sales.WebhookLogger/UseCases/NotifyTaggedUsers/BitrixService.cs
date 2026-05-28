using Gems.Sales.WebhookLogger.Bot;
using Gems.Sales.WebhookLogger.Models;
using Microsoft.Extensions.Options;

namespace Gems.Sales.WebhookLogger.UseCases.NotifyTaggedUsers
{
    public class BitrixService: IBitrixService
    {//ВОЗМОЖНО УДАЛИТЬ НАДО    
        private readonly IOptions<UsersMapOptions> _usersMapOptions;

        public BitrixService(IOptions<UsersMapOptions> usersMapOptions)
        {
            _usersMapOptions = usersMapOptions;
        }

        public string GetBitrixId(string taggedUser)
        {
            if (!string.IsNullOrEmpty(taggedUser) && _usersMapOptions.Value.Map != null)
            {
                if (_usersMapOptions.Value.Map.TryGetValue(taggedUser, out var bitrixId))
                {
                    return bitrixId;
                }
                return string.Empty;
            }
            return string.Empty;
        }
    }
}
