using Gems.Sales.Notifier.Application;
using Gems.Sales.Notifier.Infrastructure.Messaging;
using Gems.Sales.Notifier.Infrastructure.SalesManagementSystem;
using Gems.Sales.Notifier.Options;
using MediatR;
using Microsoft.Extensions.Options;
using Serilog;

namespace Gems.Sales.Notifier.UseCases.NotifyTaggedUsers
{
    internal sealed class NotifyTaggedUsersCommandHandler : IRequestHandler<NotifyTaggedUsersCommand>
    {
        private readonly IMessenger _messageSender;
        private readonly IOptions<UsersMapOptions> _usersMapOptions;
        private readonly ISalesManagementSystemClient _systemClient;
        private readonly INotificationMessageComposer _notificationMessageComposer;
        public NotifyTaggedUsersCommandHandler(
            IMessenger messageSender,
            IOptions<UsersMapOptions> usersMapOptions,
            ISalesManagementSystemClient systemClient,
            INotificationMessageComposer notificationMessageComposer)
        {
            _messageSender = messageSender;
            _usersMapOptions = usersMapOptions;
            _systemClient = systemClient;
            _notificationMessageComposer = notificationMessageComposer;
        }
        public async Task Handle(NotifyTaggedUsersCommand request, CancellationToken cancellationToken)
        {
            IReadOnlyCollection<string> maxUserIds = GetMaxId(request.UserIds);
            var deal = await _systemClient.GetDeal(request.DealId, cancellationToken);

            foreach (var maxUserId in maxUserIds)
            {
                string msgText = _notificationMessageComposer.BuildMessage(deal?.Title, request.DealId);
                await _messageSender.SendNotification(Convert.ToInt32(maxUserId), msgText);
            }
        }
        private IReadOnlyCollection<string> GetMaxId(long[] bitrixIds)
        {
            string[] bitrixUsers = bitrixIds.Select(x => x.ToString()).ToArray();
            var foundMaxIds = new List<string>();
            var keysList = new List<string>(_usersMapOptions.Value.Map.Keys);

            for (int i = 0; i < keysList.Count; i++)
            {
                string key = keysList[i];
                string value = _usersMapOptions.Value.Map[key];

                if (bitrixUsers.Contains(key))
                {
                    foundMaxIds.Add(value);
                    Log.Information($"Пользователь с битрикс id {key} найден в Максе под id {value}");
                }
            }

            foreach (var bitrixId in bitrixUsers)
            {
                if (!_usersMapOptions.Value.Map.Keys.Contains(bitrixId))
                {
                    Log.Warning($"Пользователя с битрикс id {bitrixId} нет в Максе");
                }
            }

            return foundMaxIds.AsReadOnly();
        }
    }
}
