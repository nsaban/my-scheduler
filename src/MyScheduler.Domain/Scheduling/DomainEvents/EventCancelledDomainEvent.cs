using MyScheduler.Domain.Common;

namespace MyScheduler.Domain.Scheduling.DomainEvents;

public sealed record EventCancelledDomainEvent(Guid EventId, DateTime OccurredOnUtc) : IDomainEvent;
