using System.Text;

namespace Gems.Sales.Notifier.Application
{
    internal sealed class NotificationMessageComposer : INotificationMessageComposer
    {
        public string BuildMessage(string? title)
        {
            var builder = new StringBuilder();
            builder.Append("Вы были упомянуты в сделке");

            if (!string.IsNullOrWhiteSpace(title))
            {
                builder.AppendFormat(" {0}", title);
            }

            return builder.ToString();
        }
    }
}
