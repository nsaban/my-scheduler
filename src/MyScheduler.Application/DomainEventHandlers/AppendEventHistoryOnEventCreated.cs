using MyScheduler.Application.Common;
using MyScheduler.Domain.History;
using MyScheduler.Domain.Scheduling.DomainEvents;

namespace MyScheduler.Application.DomainEventHandlers;

public sealed class AppendEventHistoryOnEventCreated(EventHistoryWriter eventHistoryWriter)
    : IDomainEventHandler<EventCreatedDomainEvent>
{
    public Task HandleAsync(EventCreatedDomainEvent domainEvent, CancellationToken cancellationToken) =>
        eventHistoryWriter.RecordAsync(domainEvent.EventId, EventChangeType.Created, domainEvent.OccurredOnUtc, cancellationToken);
}
