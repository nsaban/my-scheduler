using MyScheduler.Domain.Common;

namespace MyScheduler.Domain.ValueObjects;

public sealed record DateTimeRange
{
    public DateTime Start { get; }

    public DateTime End { get; }

    public DateTimeRange(DateTime start, DateTime end)
    {
        if (end <= start)
        {
            throw new DomainException("End must be after Start.");
        }

        Start = start;
        End = end;
    }

    public bool OverlapsWith(DateTimeRange other) => Start < other.End && other.Start < End;
}
