using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MyScheduler.Application.Abstractions;
using MyScheduler.Application.Common;
using MyScheduler.Application.DependencyInjection;
using MyScheduler.Application.UnitTests.Fakes;
using MyScheduler.Domain.Scheduling.DomainEvents;

namespace MyScheduler.Application.UnitTests.DomainEventHandlers;

public class DomainEventDispatchDiTests
{
    private static IServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance);

        // Fakes for the repository ports the domain-event-handler dependency graph needs
        // (real implementations live in Persistence, out of scope for this Application-only container).
        services.AddSingleton<IEventRepository>(new FakeEventRepository());
        services.AddSingleton<IAttendeeRepository>(new FakeAttendeeRepository());
        services.AddSingleton<IEventHistoryRepository>(new FakeEventHistoryRepository());
        services.AddSingleton<INotificationOutboxRepository>(new FakeNotificationOutboxRepository());
        services.AddSingleton<IUnitOfWork>(new FakeUnitOfWork());
        services.AddSingleton<IDateTimeProvider>(new FakeDateTimeProvider(DateTime.UtcNow));

        services.AddApplication();

        return services.BuildServiceProvider();
    }

    [Fact]
    public void EventCreated_ResolvesBothRegisteredDomainEventHandlers_ExactlyOnceEach()
    {
        var provider = BuildProvider();

        var handlers = provider
            .GetServices<INotificationHandler<DomainEventNotification<EventCreatedDomainEvent>>>()
            .ToList();

        // Exactly one MediatR notification handler for this closed generic type: the
        // DomainEventNotificationHandler<EventCreatedDomainEvent> wrapper. Two would mean the
        // open generic got registered twice (double-dispatch: duplicate history/notification rows).
        var handler = Assert.Single(handlers);

        // ...and that single wrapper must itself fan out to both concrete IDomainEventHandler<T>
        // registrations (history + notification), not silently drop one via single-service injection.
        var innerHandlers = provider.GetServices<IDomainEventHandler<EventCreatedDomainEvent>>().ToList();
        Assert.Equal(2, innerHandlers.Count);

        Assert.NotNull(handler);
    }

    [Fact]
    public void AttendeeResponseRecorded_ResolvesExactlyOneHandler()
    {
        var provider = BuildProvider();

        var handlers = provider
            .GetServices<INotificationHandler<DomainEventNotification<AttendeeResponseRecordedDomainEvent>>>()
            .ToList();

        Assert.Single(handlers);
    }
}
