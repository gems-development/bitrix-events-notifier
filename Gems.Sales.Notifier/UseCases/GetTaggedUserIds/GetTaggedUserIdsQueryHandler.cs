using Gems.Sales.Notifier.Application;
using Gems.Sales.Notifier.Infrastructure.SalesManagementSystem;
using MediatR;

namespace Gems.Sales.Notifier.UseCases.GetTaggedUserIds
{
    public class GetTaggedUserIdsQueryHandler(ISalesManagementSystemClient client, IUserIdExtractor extractor) : IRequestHandler<GetTaggedUserIdsQuery, (long DealId, long[] UserIds)?>
    {
        public async Task<(long DealId, long[] UserIds)?> Handle(GetTaggedUserIdsQuery request, CancellationToken cancellationToken)
        {
            var comment = await client.GetComment(request.commentId, cancellationToken);
            if (comment == null)
            {
                return null;
            }
            if (string.IsNullOrWhiteSpace(comment?.Body))
            {
                return null;
            }
            long[] userIds = extractor.ExtractUserIds(comment.Body);
            return (comment.DealId, UserIds: userIds);
        }
    }
}
