using Microsoft.EntityFrameworkCore;
using MyScheduler.Domain.Attendees;
using MyScheduler.Domain.Scheduling;
using MyScheduler.Domain.ValueObjects;

namespace MyScheduler.Persistence.IntegrationTests;

[Collection(nameof(SqlServerCollection))]
public class EventConcurrencyTests(SqlServerContainerFixture fixture)
{
    [Fact]
    public async Task Update_WhenAnotherUpdateWonTheRace_ThrowsConcurrencyException()
    {
        var nowUtc = DateTime.UtcNow;
        var organizer = Attendee.Create("Dr. Smith", EmailAddress.Create($"{Guid.NewGuid()}@practice.com"), nowUtc);
        var timeRange = new DateTimeRange(nowUtc.AddDays(1), nowUtc.AddDays(1).AddHours(1));
        var @event = Event.Schedule("Checkup", null, timeRange, organizer.Id, [], nowUtc);

        await using (var seedContext = fixture.CreateContext())
        {
            seedContext.Attendees.Add(organizer);
            seedContext.Events.Add(@event);
            await seedContext.SaveChangesAsync();
        }

        // Two independent contexts loading the same row, simulating two concurrent requests.
        await using var contextA = fixture.CreateContext();
        var eventA = await contextA.Events.SingleAsync(e => e.Id == @event.Id);

        await using var contextB = fixture.CreateContext();
        var eventB = await contextB.Events.SingleAsync(e => e.Id == @event.Id);

        eventA.Cancel(nowUtc.AddMinutes(1));
        await contextA.SaveChangesAsync();

        eventB.UpdateDetails("New Title", null, timeRange, nowUtc.AddMinutes(2));

        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => contextB.SaveChangesAsync());
    }
}
