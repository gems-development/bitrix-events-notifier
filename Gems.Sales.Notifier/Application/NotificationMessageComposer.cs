using System.Text;

namespace Gems.Sales.Notifier.Application
{
    internal sealed class NotificationMessageComposer : INotificationMessageComposer
    {
        public string BuildMessage(string? title, long dealId)
        {
            var builder = new StringBuilder();
            builder.Append("Вы были упомянуты в сделке");

            if (!string.IsNullOrWhiteSpace(title))
            {
                builder.AppendFormat(" {0}", title);
            }
            builder.AppendFormat(": https://crm.gemsdev.ru/crm/deal/details/{0}/", dealId);
            return builder.ToString();
        }
    }
}
