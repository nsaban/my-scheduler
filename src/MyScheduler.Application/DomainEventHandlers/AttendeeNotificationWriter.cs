using System.Text.Json;
using MyScheduler.Application.Abstractions;
using MyScheduler.Domain.Notifications;
using MyScheduler.Domain.Scheduling;

namespace MyScheduler.Application.DomainEventHandlers;

public sealed class AttendeeNotificationWriter(
    IEventRepository eventRepository,
    INotificationOutboxRepository notificationOutboxRepository)
{
    public async Task NotifyAllAttendeesAsync(
        Guid eventId,
        NotificationTriggerType triggerType,
        DateTime occurredOnUtc,
        CancellationToken cancellationToken)
    {
        var @event = await eventRepository.GetByIdAsync(eventId, cancellationToken)
            ?? throw new InvalidOperationException($"Event '{eventId}' was not found while enqueueing notifications.");

        var payload = JsonSerializer.Serialize(new
        {
            @event.Id,
            @event.Title,
            StartTimeUtc = @event.TimeRange.Start,
            EndTimeUtc = @event.TimeRange.End,
        });

        foreach (var eventAttendee in @event.EventAttendees)
        {
            var notification = Notification.Create(
                @event.Id, eventAttendee.AttendeeId, NotificationChannel.Email, triggerType, payload, occurredOnUtc);

            await notificationOutboxRepository.AddAsync(notification, cancellationToken);
        }
    }

    public async Task NotifyOrganizerAsync(
        Guid eventId,
        Guid respondingAttendeeId,
        ResponseStatus response,
        NotificationTriggerType triggerType,
        DateTime occurredOnUtc,
        CancellationToken cancellationToken)
    {
        var @event = await eventRepository.GetByIdAsync(eventId, cancellationToken)
            ?? throw new InvalidOperationException($"Event '{eventId}' was not found while enqueueing notifications.");

        var payload = JsonSerializer.Serialize(new
        {
            @event.Id,
            @event.Title,
            RespondingAttendeeId = respondingAttendeeId,
            Response = response.ToString(),
        });

        var notification = Notification.Create(
            @event.Id, @event.OrganizerId, NotificationChannel.Email, triggerType, payload, occurredOnUtc);

        await notificationOutboxRepository.AddAsync(notification, cancellationToken);
    }
}
