using Gems.Sales.Notifier.Application;
using Gems.Sales.Notifier.UseCases.GetTaggedUserIds;
using Gems.Sales.Notifier.UseCases.NotifyTaggedUsers;
using MediatR;

namespace Gems.Sales.Notifier.Infrastructure
{
    internal sealed class RequestQueueHandler : BackgroundService
    {
        private readonly IRequestQueue<long> _queue;
        private readonly ISender sender;
        public RequestQueueHandler(IRequestQueue<long> queue, ISender sender)
        {
            _queue = queue;
            this.sender = sender;
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        { 
            while(!cancellationToken.IsCancellationRequested)
            {
                if (_queue.Count > 0)
                {
                    var query = new GetTaggedUserIdsQuery(_queue.Dequeue());
                    await sender.Send(query, cancellationToken);
                    //var command = new NotifyTaggedUsersCommand([]/*_queue.Dequeue()*/);

                    //await sender.Send(command, cancellationToken);
                }

                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            }
        }
    }
}
