using MyScheduler.Application.Common;
using MyScheduler.Domain.Notifications;
using MyScheduler.Domain.Scheduling.DomainEvents;

namespace MyScheduler.Application.DomainEventHandlers;

public sealed class NotifyAttendeesOnEventUpdated(AttendeeNotificationWriter attendeeNotificationWriter)
    : IDomainEventHandler<EventUpdatedDomainEvent>
{
    public Task HandleAsync(EventUpdatedDomainEvent domainEvent, CancellationToken cancellationToken) =>
        attendeeNotificationWriter.NotifyAllAttendeesAsync(
            domainEvent.EventId, NotificationTriggerType.EventUpdated, domainEvent.OccurredOnUtc, cancellationToken);
}
