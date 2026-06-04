using Gems.Sales.Notifier.Infrastructure.SalesManagementSystem;
using MediatR;
using System.Text.RegularExpressions;

namespace Gems.Sales.Notifier.UseCases.GetTaggedUserIds
{
    public class GetTaggedUserIdsQueryHandler(ISalesManagementSystemClient client) : IRequestHandler<GetTaggedUserIdsQuery, long[]>
    {
        public async Task<long[]> Handle(GetTaggedUserIdsQuery request, CancellationToken cancellationToken)
        {
            var comment = await client.GetComment(request.commentId);
            if (string.IsNullOrWhiteSpace(comment))
            { 
                return Array.Empty<long>();
            }
            long[] userIds = ExtractUserId(comment);
            return userIds;
        }
        private long[] ExtractUserId(string comment)
        {
            return Regex.Matches(comment, @"\[USER=(\d+)\]")
        .Select(match => long.Parse(match.Groups[1].Value))
        .ToArray();
        }
    }
}
