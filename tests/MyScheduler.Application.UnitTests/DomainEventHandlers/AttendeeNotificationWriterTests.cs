using MyScheduler.Application.DomainEventHandlers;
using MyScheduler.Application.UnitTests.Fakes;
using MyScheduler.Domain.Notifications;
using MyScheduler.Domain.Scheduling;
using MyScheduler.Domain.ValueObjects;

namespace MyScheduler.Application.UnitTests.DomainEventHandlers;

public class AttendeeNotificationWriterTests
{
    private static readonly DateTime NowUtc = new(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task NotifyAllAttendeesAsync_EnqueuesOneNotificationPerInvitee()
    {
        var organizerId = Guid.NewGuid();
        var attendeeId = Guid.NewGuid();
        var @event = Event.Schedule(
            "Checkup", null, new DateTimeRange(NowUtc, NowUtc.AddHours(1)), organizerId, [attendeeId], NowUtc);

        var eventRepository = new FakeEventRepository();
        eventRepository.Events[@event.Id] = @event;
        var outbox = new FakeNotificationOutboxRepository();

        var writer = new AttendeeNotificationWriter(eventRepository, outbox);

        await writer.NotifyAllAttendeesAsync(@event.Id, NotificationTriggerType.EventCreated, NowUtc, CancellationToken.None);

        Assert.Equal(2, outbox.Notifications.Count);
        Assert.Contains(outbox.Notifications, n => n.RecipientAttendeeId == organizerId);
        Assert.Contains(outbox.Notifications, n => n.RecipientAttendeeId == attendeeId);
        Assert.All(outbox.Notifications, n => Assert.Equal(NotificationTriggerType.EventCreated, n.TriggerType));
    }

    [Fact]
    public async Task NotifyOrganizerAsync_EnqueuesSingleNotificationToOrganizer()
    {
        var organizerId = Guid.NewGuid();
        var attendeeId = Guid.NewGuid();
        var @event = Event.Schedule(
            "Checkup", null, new DateTimeRange(NowUtc, NowUtc.AddHours(1)), organizerId, [attendeeId], NowUtc);

        var eventRepository = new FakeEventRepository();
        eventRepository.Events[@event.Id] = @event;
        var outbox = new FakeNotificationOutboxRepository();

        var writer = new AttendeeNotificationWriter(eventRepository, outbox);

        await writer.NotifyOrganizerAsync(
            @event.Id, attendeeId, ResponseStatus.Accepted, NotificationTriggerType.ResponseRecorded, NowUtc, CancellationToken.None);

        var notification = Assert.Single(outbox.Notifications);
        Assert.Equal(organizerId, notification.RecipientAttendeeId);
        Assert.Equal(NotificationTriggerType.ResponseRecorded, notification.TriggerType);
    }
}
