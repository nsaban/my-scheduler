using MyScheduler.Application.Common;
using MyScheduler.Domain.History;
using MyScheduler.Domain.Scheduling.DomainEvents;

namespace MyScheduler.Application.DomainEventHandlers;

public sealed class AppendEventHistoryOnEventCancelled(EventHistoryWriter eventHistoryWriter)
    : IDomainEventHandler<EventCancelledDomainEvent>
{
    public Task HandleAsync(EventCancelledDomainEvent domainEvent, CancellationToken cancellationToken) =>
        eventHistoryWriter.RecordAsync(domainEvent.EventId, EventChangeType.Cancelled, domainEvent.OccurredOnUtc, cancellationToken);
}
