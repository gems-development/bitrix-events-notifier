using MAX.Bot.Interfaces;
using MAX.Bot.Interfaces.Models.Request.Message;

namespace Gems.Sales.Notifier.Infrastructure.Messaging
{
    internal sealed class MaxMessenger : IMessenger
    {
        private readonly IMaxBotClient _botClient;

        public MaxMessenger(IMaxBotClient botClient)
        {
            _botClient = botClient;
        }

        public Task SendMessage(long chatId, string message, CancellationToken cancellationToken)
        {
            return _botClient.SendMessageAsync(
                new SendMessageRequest
                {
                    UserId = chatId,
                    Text = message
                },
                cancellationToken);
        }
    }
}
