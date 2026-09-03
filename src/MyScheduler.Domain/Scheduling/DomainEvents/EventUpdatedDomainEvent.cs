using MyScheduler.Domain.Common;

namespace MyScheduler.Domain.Scheduling.DomainEvents;

public sealed record EventUpdatedDomainEvent(Guid EventId, DateTime OccurredOnUtc) : IDomainEvent;
