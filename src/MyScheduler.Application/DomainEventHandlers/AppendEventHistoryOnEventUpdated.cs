using MyScheduler.Application.Common;
using MyScheduler.Domain.History;
using MyScheduler.Domain.Scheduling.DomainEvents;

namespace MyScheduler.Application.DomainEventHandlers;

public sealed class AppendEventHistoryOnEventUpdated(EventHistoryWriter eventHistoryWriter)
    : IDomainEventHandler<EventUpdatedDomainEvent>
{
    public Task HandleAsync(EventUpdatedDomainEvent domainEvent, CancellationToken cancellationToken) =>
        eventHistoryWriter.RecordAsync(domainEvent.EventId, EventChangeType.Updated, domainEvent.OccurredOnUtc, cancellationToken);
}
