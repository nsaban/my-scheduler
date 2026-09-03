using Microsoft.EntityFrameworkCore;
using MyScheduler.Domain.Attendees;
using MyScheduler.Domain.History;
using MyScheduler.Domain.Notifications;
using MyScheduler.Domain.Scheduling;
using MyScheduler.Domain.ValueObjects;

namespace MyScheduler.Persistence.IntegrationTests;

[Collection(nameof(SqlServerCollection))]
public class JsonCheckConstraintTests(SqlServerContainerFixture fixture)
{
    // Domain.EventHistory.Record / Notification.Create only guard against a blank payload, not malformed
    // JSON — the ISJSON(...) CHECK constraint is the defense-in-depth layer that catches that case.
    [Fact]
    public async Task InsertEventHistory_WhenSnapshotIsNotJson_ViolatesCheckConstraint()
    {
        var (organizer, @event) = await SeedEventAsync();

        await using var context = fixture.CreateContext();
        context.EventHistory.Add(EventHistory.Record(
            @event.Id, 1, EventChangeType.Created, "not-json", organizer.Id, DateTime.UtcNow));

        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }

    [Fact]
    public async Task InsertNotification_WhenPayloadIsNotJson_ViolatesCheckConstraint()
    {
        var (organizer, @event) = await SeedEventAsync();

        await using var context = fixture.CreateContext();
        context.Notifications.Add(Notification.Create(
            @event.Id, organizer.Id, NotificationChannel.Email, NotificationTriggerType.EventCreated, "not-json", DateTime.UtcNow));

        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }

    private async Task<(Attendee Organizer, Event Event)> SeedEventAsync()
    {
        var nowUtc = DateTime.UtcNow;
        var organizer = Attendee.Create("Dr. Smith", EmailAddress.Create($"{Guid.NewGuid()}@practice.com"), nowUtc);
        var timeRange = new DateTimeRange(nowUtc.AddDays(1), nowUtc.AddDays(1).AddHours(1));
        var @event = Event.Schedule("Checkup", null, timeRange, organizer.Id, [], nowUtc);

        await using var context = fixture.CreateContext();
        context.Attendees.Add(organizer);
        context.Events.Add(@event);
        await context.SaveChangesAsync();

        return (organizer, @event);
    }
}
