using MyScheduler.Application.Abstractions;

namespace MyScheduler.Application.UnitTests.Fakes;

public sealed class FakeDateTimeProvider(DateTime utcNow) : IDateTimeProvider
{
    public DateTime UtcNow { get; set; } = utcNow;
}
