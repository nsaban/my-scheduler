using Microsoft.EntityFrameworkCore;
using MyScheduler.Domain.Attendees;
using MyScheduler.Domain.ValueObjects;

namespace MyScheduler.Persistence.IntegrationTests;

[Collection(nameof(SqlServerCollection))]
public class AttendeeEmailUniquenessTests(SqlServerContainerFixture fixture)
{
    [Fact]
    public async Task Insert_WhenEmailDiffersOnlyByCase_ViolatesUniqueIndex()
    {
        var nowUtc = DateTime.UtcNow;
        var email = $"dr.{Guid.NewGuid():N}@practice.com";

        await using var context = fixture.CreateContext();
        context.Attendees.Add(Attendee.Create("Dr. Smith", EmailAddress.Create(email), nowUtc));
        await context.SaveChangesAsync();

        context.Attendees.Add(Attendee.Create("Dr. Smith Duplicate", EmailAddress.Create(email.ToUpperInvariant()), nowUtc));

        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }
}
