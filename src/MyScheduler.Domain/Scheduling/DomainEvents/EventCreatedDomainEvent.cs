using MyScheduler.Domain.Common;

namespace MyScheduler.Domain.Scheduling.DomainEvents;

public sealed record EventCreatedDomainEvent(Guid EventId, DateTime OccurredOnUtc) : IDomainEvent;
