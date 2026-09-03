using MyScheduler.Application.Common;
using MyScheduler.Domain.Notifications;
using MyScheduler.Domain.Scheduling.DomainEvents;

namespace MyScheduler.Application.DomainEventHandlers;

public sealed class NotifyOrganizerOnAttendeeResponseRecorded(AttendeeNotificationWriter attendeeNotificationWriter)
    : IDomainEventHandler<AttendeeResponseRecordedDomainEvent>
{
    public Task HandleAsync(AttendeeResponseRecordedDomainEvent domainEvent, CancellationToken cancellationToken) =>
        attendeeNotificationWriter.NotifyOrganizerAsync(
            domainEvent.EventId,
            domainEvent.AttendeeId,
            domainEvent.ResponseStatus,
            NotificationTriggerType.ResponseRecorded,
            domainEvent.OccurredOnUtc,
            cancellationToken);
}
