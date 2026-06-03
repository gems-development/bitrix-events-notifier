using Gems.Sales.Notifier.Infrastructure.SalesManagementSystem;
using Gems.Sales.Notifier.UseCases.NotifyTaggedUsers;
using MediatR;

namespace Gems.Sales.Notifier.UseCases.GetTaggedUserIds
{
    public class GetTaggedUserIdsQueryHandler(ISalesManagementSystemClient client) : IRequestHandler<GetTaggedUserIdsQuery, long[]>
    {
        public async Task<long[]> Handle(GetTaggedUserIdsQuery request, CancellationToken cancellationToken)
        {
            //Изменить в битриксклиенте метод
            await client.GetComment(request.commentId);
            return new long[0];
        }
    }
}
