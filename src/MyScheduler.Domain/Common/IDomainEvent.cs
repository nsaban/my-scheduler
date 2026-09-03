namespace MyScheduler.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}
