using Gems.Sales.Notifier.Application;
using Gems.Sales.Notifier.UseCases.GetTaggedUserIds;
using Gems.Sales.Notifier.UseCases.NotifyTaggedUsers;
using MediatR;

namespace Gems.Sales.Notifier.Infrastructure
{
    internal sealed class RequestQueueHandler : BackgroundService
    {
        private readonly IRequestQueue<long> _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        public RequestQueueHandler(IRequestQueue<long> queue, IServiceScopeFactory scopeFactory)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        { 
            while(!cancellationToken.IsCancellationRequested)
            {
                if (_queue.Count > 0)
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
                        var query = new GetTaggedUserIdsQuery(_queue.Dequeue());
                        var bitrixIds = await sender.Send(query, cancellationToken);
                        var command = new NotifyTaggedUsersCommand(bitrixIds);

                        await sender.Send(command, cancellationToken);
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            }
        }
    }
}
