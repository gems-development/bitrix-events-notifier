using MediatR;

namespace Gems.Sales.Notifier.UseCases.NotifyTaggedUsers
{
    public record NotifyTaggedUsersCommand(int[] bitrixUserIds) : IRequest;
}
