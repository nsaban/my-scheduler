using MyScheduler.Domain.Common;
using MyScheduler.Domain.ValueObjects;

namespace MyScheduler.Domain.UnitTests.ValueObjects;

public class DateTimeRangeTests
{
    [Fact]
    public void Constructor_WhenEndAfterStart_Succeeds()
    {
        var start = new DateTime(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);
        var end = start.AddHours(1);

        var range = new DateTimeRange(start, end);

        Assert.Equal(start, range.Start);
        Assert.Equal(end, range.End);
    }

    [Theory]
    [MemberData(nameof(InvalidRanges))]
    public void Constructor_WhenEndNotAfterStart_Throws(DateTime start, DateTime end)
    {
        Assert.Throws<DomainException>(() => new DateTimeRange(start, end));
    }

    public static TheoryData<DateTime, DateTime> InvalidRanges()
    {
        var start = new DateTime(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);

        return new TheoryData<DateTime, DateTime>
        {
            { start, start },
            { start, start.AddMinutes(-1) },
        };
    }

    [Fact]
    public void OverlapsWith_WhenRangesOverlap_ReturnsTrue()
    {
        var start = new DateTime(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);
        var first = new DateTimeRange(start, start.AddHours(2));
        var second = new DateTimeRange(start.AddHours(1), start.AddHours(3));

        Assert.True(first.OverlapsWith(second));
        Assert.True(second.OverlapsWith(first));
    }

    [Fact]
    public void OverlapsWith_WhenRangesDoNotOverlap_ReturnsFalse()
    {
        var start = new DateTime(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);
        var first = new DateTimeRange(start, start.AddHours(1));
        var second = new DateTimeRange(start.AddHours(1), start.AddHours(2));

        Assert.False(first.OverlapsWith(second));
        Assert.False(second.OverlapsWith(first));
    }
}
