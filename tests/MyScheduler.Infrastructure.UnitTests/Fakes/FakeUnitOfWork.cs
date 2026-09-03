using MyScheduler.Application.Abstractions;

namespace MyScheduler.Infrastructure.UnitTests.Fakes;

public sealed class FakeUnitOfWork : IUnitOfWork
{
    public int SaveChangesCallCount { get; private set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCallCount++;
        return Task.FromResult(0);
    }
}
