using MediatR;

namespace Gems.Sales.Notifier.UseCases.NotifyTaggedUsers
{
    public record NotifyTaggedUsersCommand(long[] bitrixUserIds) : IRequest;
}
