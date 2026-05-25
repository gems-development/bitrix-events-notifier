using Gems.Sales.WebhookLogger.Models;
using MAX.Bot.Interfaces;
using MAX.Bot.Interfaces.Models;
using MAX.Bot.Interfaces.Models.Request.Message;
using Microsoft.Extensions.Options;
using Serilog;
using System.Text.RegularExpressions;

namespace Gems.Sales.WebhookLogger.Bot
{
    public class BotService : IMessenger
    {
        private readonly IMaxBotClient _botClient;

        public BotService(IMaxBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task StartBot(IOptions<UsersMapOptions> usersMapOptions)
        {
            // Получение обновлений
            _ = _botClient.PollUpdatesWithCallback(
                async (update, client) =>
                {
                    if (update is MessageCreatedUpdate messageCreated)
                    {
                        Log.Information($"Получено сообщение: {messageCreated.Message?.Body?.Text}");
                        string? msgText = messageCreated.Message?.Body?.Text;
                        switch(msgText)
                        {
                            //Пользователь отправил /start
                            case "/start":
                                await SendWelcomeMessage(Convert.ToInt32(usersMapOptions));
                                break;
                            //Пользователь отправил тег например @user1
                            case string text when text.Contains("@"):
                                var tagMatch = Regex.Match(msgText, @"@\w+");
                                if (tagMatch.Success)
                                {
                                    string tag = tagMatch.Value;
                                    string nickname = tag.Trim('@');
                                    string bitrixId = GetBitrixId(nickname, usersMapOptions);
                                    if (!string.IsNullOrEmpty(bitrixId))
                                    {
                                        Log.Information($"Найден битрикс пользователя {nickname}");
                                        await SendNotification(Convert.ToInt32(tag));
                                    }
                                    else
                                    {
                                        Log.Information($"Битрикс пользователя {nickname} не найден");
                                    }
                                }
                                break;
                        }
                    }
                },
                limit: 100,
                timeout: 90,
                types: new List<string> { UpdateTypes.MessageCreated });
        }
        //Метод для отправки приветственного сообщения
        public async Task SendWelcomeMessage(long chatId)
        {
            await _botClient.SendMessageAsync(new SendMessageRequest
            {
                ChatId = chatId,
                Text = "Добро пожаловать!"
            });
            Log.Information($"Отправлено приветственное сообщение для {chatId}");
        }
        //Метод для отправки приветственного сообщения
        public async Task SendNotification(long chatId)
        {
            await _botClient.SendMessageAsync(new SendMessageRequest
            {
                ChatId = chatId,
                Text = "Вы упомянуты в лиде"
            });
            Log.Information($"Отправлено уведомление для {chatId}");
        }
        //Метод для проверки наличия пользователя макса в битрикс (Пока что сравнение не id макса, а тега)
        private static string GetBitrixId(string taggedUser, IOptions<UsersMapOptions> options)
        {
            if (!string.IsNullOrEmpty(taggedUser) && options.Value.Map != null)
            {
                if (options.Value.Map.TryGetValue(taggedUser, out var bitrixId))
                {
                    return bitrixId;
                }
                return string.Empty;
            }
            return string.Empty;
        }
        //Test for GetBitrixId method(можно удалить)
        public static string TestGetBitrixId(IOptions<UsersMapOptions> usersMapOptions)
        {
            string? msgText = "Hello, @USER20_CHAT_ID, howdau?";
            switch (msgText)
            {
                //Пользователь отправил тег например @user1
                case string text when text.Contains("@"):
                    var tagMatch = Regex.Match(msgText, @"@\w+");
                    if (tagMatch.Success)
                    {
                        string tag = tagMatch.Value;
                        string nickname = tag.Trim('@');
                        string bitrixId = GetBitrixId(nickname, usersMapOptions);
                        if (!string.IsNullOrEmpty(bitrixId)) { return bitrixId; }
                        return string.Empty;
                    }
                    return string.Empty;
            }
            return string.Empty;
        }
    }
}
