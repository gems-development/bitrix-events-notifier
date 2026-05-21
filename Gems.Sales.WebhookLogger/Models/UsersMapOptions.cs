using Microsoft.Extensions.Options;

namespace Gems.Sales.WebhookLogger.Models
{
    public class UsersMapOptions
    {

        public const string SectionName = "UsersMap";
        public required Dictionary<string, string> Map { get; set; }
        //Тест на привязку к конфигу (для себя, можно удалить)
        public static void Test(IOptions<UsersMapOptions> options)
        {
            try
            {
                bool bl = options.Value.Map.TryGetValue("USER1_CHAT_ID", out var maxId);
                Console.WriteLine(bl);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
