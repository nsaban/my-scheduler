using MyScheduler.Domain.Common;
using MyScheduler.Domain.History;

namespace MyScheduler.Domain.UnitTests.History;

public class EventHistoryTests
{
    private static readonly DateTime NowUtc = new(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Record_WhenValid_Succeeds()
    {
        var eventId = Guid.NewGuid();

        var history = EventHistory.Record(eventId, 1, EventChangeType.Created, "{}", null, NowUtc);

        Assert.Equal(eventId, history.EventId);
        Assert.Equal(1, history.Version);
        Assert.Equal(EventChangeType.Created, history.ChangeType);
        Assert.Equal("{}", history.Snapshot);
        Assert.Null(history.ChangedByAttendeeId);
        Assert.Equal(NowUtc, history.ChangedAtUtc);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Record_WhenSnapshotMissing_Throws(string snapshot)
    {
        Assert.Throws<DomainException>(() =>
            EventHistory.Record(Guid.NewGuid(), 1, EventChangeType.Created, snapshot, null, NowUtc));
    }
}
