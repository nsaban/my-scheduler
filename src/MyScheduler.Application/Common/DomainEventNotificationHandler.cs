using MediatR;
using MyScheduler.Domain.Common;

namespace MyScheduler.Application.Common;

public sealed class DomainEventNotificationHandler<TDomainEvent>(IEnumerable<IDomainEventHandler<TDomainEvent>> handlers)
    : INotificationHandler<DomainEventNotification<TDomainEvent>>
    where TDomainEvent : IDomainEvent
{
    public async Task Handle(DomainEventNotification<TDomainEvent> notification, CancellationToken cancellationToken)
    {
        foreach (var handler in handlers)
        {
            await handler.HandleAsync(notification.DomainEvent, cancellationToken);
        }
    }
}
