using MediatR;

namespace Gems.Sales.Notifier.UseCases.NotifyTaggedUsers
{
    public record NotifyTaggedUsersCommand(long DealId, long[] UserIds) : IRequest;
}
