using MediatR;
using MyScheduler.Application.Abstractions;
using MyScheduler.Application.Common;
using MyScheduler.Domain.Common;

namespace MyScheduler.Persistence;

public sealed class UnitOfWork(AppDbContext dbContext, IPublisher publisher) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        await DispatchDomainEventsAsync(cancellationToken);

        return await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var aggregatesWithEvents = dbContext.ChangeTracker
            .Entries<AggregateRoot>()
            .Select(entry => entry.Entity)
            .Where(aggregate => aggregate.DomainEvents.Count > 0)
            .ToList();

        foreach (var aggregate in aggregatesWithEvents)
        {
            var domainEvents = aggregate.DomainEvents.ToList();
            aggregate.ClearDomainEvents();

            foreach (var domainEvent in domainEvents)
            {
                await publisher.Publish(WrapForMediatR(domainEvent), cancellationToken);
            }
        }
    }

    private static INotification WrapForMediatR(IDomainEvent domainEvent)
    {
        var wrapperType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
        return (INotification)Activator.CreateInstance(wrapperType, domainEvent)!;
    }
}
