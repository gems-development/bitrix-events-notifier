using MediatR;

namespace Gems.Sales.Notifier.UseCases.GetTaggedUserIds
{
    public record GetTaggedUserIdsQuery(long commentId) : IRequest<(long DealId, long[] UserIds)?>;
}
