using Gems.Sales.WebhookLogger.Bot;
using Gems.Sales.WebhookLogger.UseCases.NotifyTaggedUsers;
using Serilog;
using System.Text.RegularExpressions;

namespace Gems.Sales.WebhookLogger.Handlers
{
    public class MessageHandler: IMessageHandler
    {
        private readonly IBitrixService _bitrixService;
        private readonly IMessenger _messageSender;

        public MessageHandler(IBitrixService bitrixService, IMessenger messageSender)
        {
            _bitrixService = bitrixService;
            _messageSender = messageSender;
        }

        public async Task HandleAsync(string? messageText)
        {
            if (string.IsNullOrEmpty(messageText)) return;

            //Обработка команд
            if (messageText == "/start")
            {
                await _messageSender.SendWelcomeMessage(12345/*Нужно получить айди чата, в котором отправили старт*/);
            }

            else if(messageText.Contains("@"))
            {
                var tagMatch = Regex.Match(messageText, @"@\w+");
                if (tagMatch.Success)
                {
                    string tag = tagMatch.Value;
                    string nickname = tag.Trim('@');
                    string bitrixId =_bitrixService.GetBitrixId(nickname);
                    if (!string.IsNullOrEmpty(bitrixId))
                    {
                        Log.Information($"Найден битрикс пользователя {nickname}");
                        await _messageSender.SendNotification(Convert.ToInt32(tag));
                    }
                    else
                    {
                        Log.Information($"Битрикс пользователя {nickname} не найден");
                    }
                }
            }
        }
    }
}
