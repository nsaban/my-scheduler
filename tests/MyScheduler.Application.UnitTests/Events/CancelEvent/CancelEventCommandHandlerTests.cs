using MyScheduler.Application.Common;
using MyScheduler.Application.Events.CancelEvent;
using MyScheduler.Application.UnitTests.Fakes;
using MyScheduler.Domain.Scheduling;
using MyScheduler.Domain.ValueObjects;

namespace MyScheduler.Application.UnitTests.Events.CancelEvent;

public class CancelEventCommandHandlerTests
{
    private static readonly DateTime NowUtc = new(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task Handle_WhenEventExists_CancelsAndSaves()
    {
        var @event = Event.Schedule("Checkup", null, new DateTimeRange(NowUtc, NowUtc.AddHours(1)), Guid.NewGuid(), [], NowUtc);
        var eventRepository = new FakeEventRepository();
        eventRepository.Events[@event.Id] = @event;
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CancelEventCommandHandler(eventRepository, new FakeDateTimeProvider(NowUtc.AddMinutes(5)), unitOfWork);

        await handler.Handle(new CancelEventCommand(@event.Id, [1]), CancellationToken.None);

        Assert.Equal(EventStatus.Cancelled, @event.Status);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_WhenEventDoesNotExist_ThrowsNotFoundException()
    {
        var handler = new CancelEventCommandHandler(
            new FakeEventRepository(), new FakeDateTimeProvider(NowUtc), new FakeUnitOfWork());

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new CancelEventCommand(Guid.NewGuid(), [1]), CancellationToken.None));
    }
}
