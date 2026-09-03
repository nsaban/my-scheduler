using MyScheduler.Application.Common;
using MyScheduler.Application.Events.UpdateEvent;
using MyScheduler.Application.UnitTests.Fakes;
using MyScheduler.Domain.Scheduling;
using MyScheduler.Domain.ValueObjects;

namespace MyScheduler.Application.UnitTests.Events.UpdateEvent;

public class UpdateEventCommandHandlerTests
{
    private static readonly DateTime NowUtc = new(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task Handle_WhenEventExists_UpdatesDetailsAndSaves()
    {
        var organizerId = Guid.NewGuid();
        var @event = Event.Schedule("Checkup", null, new DateTimeRange(NowUtc, NowUtc.AddHours(1)), organizerId, [], NowUtc);
        var eventRepository = new FakeEventRepository();
        eventRepository.Events[@event.Id] = @event;
        var unitOfWork = new FakeUnitOfWork();

        var handler = new UpdateEventCommandHandler(eventRepository, new FakeDateTimeProvider(NowUtc.AddMinutes(5)), unitOfWork);

        var newStart = NowUtc.AddDays(1);
        var command = new UpdateEventCommand(@event.Id, "New Title", "New notes", newStart, newStart.AddHours(1), [1]);

        await handler.Handle(command, CancellationToken.None);

        Assert.Equal("New Title", @event.Title);
        Assert.Equal(newStart, @event.TimeRange.Start);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_WhenEventDoesNotExist_ThrowsNotFoundException()
    {
        var handler = new UpdateEventCommandHandler(
            new FakeEventRepository(), new FakeDateTimeProvider(NowUtc), new FakeUnitOfWork());

        var command = new UpdateEventCommand(Guid.NewGuid(), "Title", null, NowUtc, NowUtc.AddHours(1), [1]);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
