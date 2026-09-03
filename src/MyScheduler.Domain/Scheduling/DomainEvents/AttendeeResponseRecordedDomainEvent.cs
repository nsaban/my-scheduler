using MyScheduler.Domain.Common;

namespace MyScheduler.Domain.Scheduling.DomainEvents;

public sealed record AttendeeResponseRecordedDomainEvent(
    Guid EventId,
    Guid AttendeeId,
    ResponseStatus ResponseStatus,
    DateTime OccurredOnUtc) : IDomainEvent;
