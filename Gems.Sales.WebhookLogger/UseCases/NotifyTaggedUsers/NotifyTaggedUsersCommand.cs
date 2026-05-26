using MediatR;

namespace Gems.Sales.WebhookLogger.UseCases.NotifyTaggedUsers
{
    public record NotifyTaggedUsersCommand(List<string> UserIds) : IRequest;

}
