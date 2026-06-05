using Gems.Sales.Notifier.Application;
using Gems.Sales.Notifier.Infrastructure.SalesManagementSystem;
using MediatR;

namespace Gems.Sales.Notifier.UseCases.GetTaggedUserIds
{
    public class GetTaggedUserIdsQueryHandler(ISalesManagementSystemClient client, IUserIdExtractor extractor) : IRequestHandler<GetTaggedUserIdsQuery, long[]>
    {

        public async Task<long[]> Handle(GetTaggedUserIdsQuery request, CancellationToken cancellationToken)
        {
            var comment = await client.GetComment(request.commentId, cancellationToken);
            if (string.IsNullOrWhiteSpace(comment))
            { 
                return Array.Empty<long>();
            }
            long[] userIds = extractor.ExtractUserIds(comment);
            return userIds;
        }
    }
}
