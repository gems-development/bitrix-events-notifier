using MediatR;

namespace Gems.Sales.WebhookLogger.UseCases.NotifyTaggedUsers
{
    public record NotifyTaggedUsersCommand(int[] bitrixUserIds) : IRequest;

}
