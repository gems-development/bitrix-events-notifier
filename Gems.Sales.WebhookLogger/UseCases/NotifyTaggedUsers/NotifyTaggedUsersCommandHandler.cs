using Consul;
using Gems.Sales.WebhookLogger.Bot;
using Gems.Sales.WebhookLogger.Models;
using MediatR;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Options;
using Serilog;

namespace Gems.Sales.WebhookLogger.UseCases.NotifyTaggedUsers
{
    public class NotifyTaggedUsersCommandHandler : IRequestHandler<NotifyTaggedUsersCommand>
    {
        private readonly IMessenger _messageSender;
        private readonly IOptions<UsersMapOptions> _usersMapOptions;
        public NotifyTaggedUsersCommandHandler(IMessenger messageSender, IOptions<UsersMapOptions> usersMapOptions)
        {
            _messageSender = messageSender;
            _usersMapOptions = usersMapOptions;
        }
        public async Task Handle(NotifyTaggedUsersCommand request, CancellationToken cancellationToken)
        {
            List<string> maxIds = GetMaxId(request.bitrixUserIds);
            foreach (var maxUserId in maxIds)
            {
                foreach (var bitrixUserId in request.bitrixUserIds)
                {
                    if (maxUserId == bitrixUserId.ToString())
                    {
                        string msgText = "Вы были упомянуты в сделке";
                        await _messageSender.SendNotification(Convert.ToInt32(maxUserId), msgText);
                        Log.Information($"Пользователь с битрикс id {bitrixUserId} найден в Максе под id {maxUserId} и ему отправлено уведомление");
                    }
                    else
                    {
                        Log.Warning($"Пользователя с битрикс id {bitrixUserId} нет в Максе");
                    }
                }
            }
            throw new NotImplementedException();
        }
        private List<string> GetMaxId(int[] bitrixIds)
        {
            string[] bitrixUsers = Array.ConvertAll(bitrixIds, x => x.ToString());
            var foundMaxIds = new List<string>();

            // Получаем список ключей для доступа по индексу
            var keysList = new List<string>(_usersMapOptions.Value.Map.Keys);

            for (int i = 0; i < keysList.Count; i++)
            {
                string key = keysList[i];
                string value = _usersMapOptions.Value.Map[key];

                // Ищем совпадение с любым из целевых значений
                if (Array.Exists(bitrixUsers, v => v == value))
                {
                    foundMaxIds.Add(key);
                }
            }
            return foundMaxIds;
        }
    }
}
