using MyScheduler.Application.Events.CheckAttendeeAvailability;
using MyScheduler.Application.UnitTests.Fakes;

namespace MyScheduler.Application.UnitTests.Events.CheckAttendeeAvailability;

public class CheckAttendeeAvailabilityQueryHandlerTests
{
    private static readonly DateTime NowUtc = new(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_ReturnsQueriesResult(bool isAvailable)
    {
        var queries = new FakeEventListingQueries { IsAvailableResult = isAvailable };
        var handler = new CheckAttendeeAvailabilityQueryHandler(queries);

        var result = await handler.Handle(
            new CheckAttendeeAvailabilityQuery(Guid.NewGuid(), NowUtc, NowUtc.AddHours(1)),
            CancellationToken.None);

        Assert.Equal(isAvailable, result);
    }
}
