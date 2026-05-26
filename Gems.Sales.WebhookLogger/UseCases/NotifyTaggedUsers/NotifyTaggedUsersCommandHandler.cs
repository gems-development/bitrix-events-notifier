using MediatR;

namespace Gems.Sales.WebhookLogger.UseCases.NotifyTaggedUsers
{
    public class NotifyTaggedUsersCommandHandler : IRequestHandler<NotifyTaggedUsersCommand>
    {
        public Task Handle(NotifyTaggedUsersCommand request, CancellationToken cancellationToken)
        {
            //Через конструкцию получить IOPtions и IMessenger 2.На основанни реквеста вытащить из опшионс сопоставление с макс айди 3. Составить шаблон сообщения 4. отправить всем кто упомянут это соо 5. На ур варнинг зажурналировать всех кого нет
            throw new NotImplementedException();
        }
    }
}
