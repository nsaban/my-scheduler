using Microsoft.EntityFrameworkCore;
using MyScheduler.Domain.Attendees;
using MyScheduler.Domain.Scheduling;
using MyScheduler.Domain.ValueObjects;

namespace MyScheduler.Persistence.IntegrationTests;

[Collection(nameof(SqlServerCollection))]
public class EventRoundTripTests(SqlServerContainerFixture fixture)
{
    [Fact]
    public async Task SaveAndReload_PersistsTimeRangeAndAttendees()
    {
        var nowUtc = DateTime.UtcNow;
        var organizer = Attendee.Create("Dr. Smith", EmailAddress.Create($"{Guid.NewGuid()}@practice.com"), nowUtc);
        var invitee = Attendee.Create("Patient Jones", EmailAddress.Create($"{Guid.NewGuid()}@practice.com"), nowUtc);
        var timeRange = new DateTimeRange(nowUtc.AddDays(1), nowUtc.AddDays(1).AddHours(1));
        var @event = Event.Schedule(
            "Annual Checkup", "Routine checkup", timeRange, organizer.Id, [invitee.Id], nowUtc);

        await using (var writeContext = fixture.CreateContext())
        {
            writeContext.Attendees.AddRange(organizer, invitee);
            writeContext.Events.Add(@event);
            await writeContext.SaveChangesAsync();
        }

        await using var readContext = fixture.CreateContext();
        var reloaded = await readContext.Events
            .Include(e => e.EventAttendees)
            .SingleAsync(e => e.Id == @event.Id);

        Assert.Equal("Annual Checkup", reloaded.Title);
        Assert.Equal(timeRange.Start, reloaded.TimeRange.Start);
        Assert.Equal(timeRange.End, reloaded.TimeRange.End);
        Assert.Equal(EventStatus.Scheduled, reloaded.Status);
        Assert.Equal(2, reloaded.EventAttendees.Count);
        Assert.Contains(reloaded.EventAttendees, ea => ea.AttendeeId == organizer.Id);
        Assert.Contains(reloaded.EventAttendees, ea => ea.AttendeeId == invitee.Id);
    }
}
