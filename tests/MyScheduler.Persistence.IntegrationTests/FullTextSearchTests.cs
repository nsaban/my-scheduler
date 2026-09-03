using Microsoft.EntityFrameworkCore;
using MyScheduler.Domain.Attendees;
using MyScheduler.Domain.Scheduling;
using MyScheduler.Domain.ValueObjects;

namespace MyScheduler.Persistence.IntegrationTests;

[Collection(nameof(SqlServerCollection))]
public class FullTextSearchTests(SqlServerContainerFixture fixture)
{
    [Fact]
    public async Task Search_WhenTitleMatchesSearchTerm_ReturnsEvent()
    {
        var nowUtc = DateTime.UtcNow;
        var organizer = Attendee.Create("Dr. Smith", EmailAddress.Create($"{Guid.NewGuid()}@practice.com"), nowUtc);
        var timeRange = new DateTimeRange(nowUtc.AddDays(1), nowUtc.AddDays(1).AddHours(1));
        var searchToken = $"Xylophone{Guid.NewGuid():N}";
        var @event = Event.Schedule($"{searchToken} Checkup", "Routine annual visit", timeRange, organizer.Id, [], nowUtc);

        await using (var context = fixture.CreateContext())
        {
            context.Attendees.Add(organizer);
            context.Events.Add(@event);
            await context.SaveChangesAsync();
        }

        // SQL Server's full-text index populates asynchronously after insert; poll briefly rather than assume it's instant.
        Event? found = null;
        for (var attempt = 0; attempt < 20 && found is null; attempt++)
        {
            await using var context = fixture.CreateContext();
            found = await context.Events
                .Where(e => EF.Functions.Contains(e.Title, searchToken))
                .SingleOrDefaultAsync();

            if (found is null)
            {
                await Task.Delay(250);
            }
        }

        Assert.NotNull(found);
        Assert.Equal(@event.Id, found!.Id);
    }
}
