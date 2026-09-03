namespace MyScheduler.Application.Abstractions;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
