using MyScheduler.Application.Common;
using MyScheduler.Domain.Notifications;
using MyScheduler.Domain.Scheduling.DomainEvents;

namespace MyScheduler.Application.DomainEventHandlers;

public sealed class NotifyAttendeesOnEventCancelled(AttendeeNotificationWriter attendeeNotificationWriter)
    : IDomainEventHandler<EventCancelledDomainEvent>
{
    public Task HandleAsync(EventCancelledDomainEvent domainEvent, CancellationToken cancellationToken) =>
        attendeeNotificationWriter.NotifyAllAttendeesAsync(
            domainEvent.EventId, NotificationTriggerType.EventCancelled, domainEvent.OccurredOnUtc, cancellationToken);
}
