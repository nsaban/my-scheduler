using MyScheduler.Application.Abstractions;

namespace MyScheduler.Infrastructure.UnitTests.Fakes;

public sealed class FakeDateTimeProvider(DateTime utcNow) : IDateTimeProvider
{
    public DateTime UtcNow { get; set; } = utcNow;
}
