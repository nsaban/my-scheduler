using MyScheduler.Application.Common;
using MyScheduler.Domain.Notifications;
using MyScheduler.Domain.Scheduling.DomainEvents;

namespace MyScheduler.Application.DomainEventHandlers;

public sealed class NotifyAttendeesOnEventCreated(AttendeeNotificationWriter attendeeNotificationWriter)
    : IDomainEventHandler<EventCreatedDomainEvent>
{
    public Task HandleAsync(EventCreatedDomainEvent domainEvent, CancellationToken cancellationToken) =>
        attendeeNotificationWriter.NotifyAllAttendeesAsync(
            domainEvent.EventId, NotificationTriggerType.EventCreated, domainEvent.OccurredOnUtc, cancellationToken);
}
