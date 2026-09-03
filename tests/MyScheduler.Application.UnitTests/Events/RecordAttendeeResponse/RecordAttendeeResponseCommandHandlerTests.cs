using MyScheduler.Application.Common;
using MyScheduler.Application.Events.RecordAttendeeResponse;
using MyScheduler.Application.UnitTests.Fakes;
using MyScheduler.Domain.Scheduling;
using MyScheduler.Domain.ValueObjects;

namespace MyScheduler.Application.UnitTests.Events.RecordAttendeeResponse;

public class RecordAttendeeResponseCommandHandlerTests
{
    private static readonly DateTime NowUtc = new(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task Handle_WhenAttendeeInvited_RecordsResponseAndSaves()
    {
        var attendeeId = Guid.NewGuid();
        var @event = Event.Schedule(
            "Checkup", null, new DateTimeRange(NowUtc, NowUtc.AddHours(1)), Guid.NewGuid(), [attendeeId], NowUtc);
        var eventRepository = new FakeEventRepository();
        eventRepository.Events[@event.Id] = @event;
        var unitOfWork = new FakeUnitOfWork();

        var handler = new RecordAttendeeResponseCommandHandler(eventRepository, new FakeDateTimeProvider(NowUtc.AddMinutes(5)), unitOfWork);

        var command = new RecordAttendeeResponseCommand(@event.Id, attendeeId, ResponseStatus.Accepted, [1]);

        await handler.Handle(command, CancellationToken.None);

        var eventAttendee = Assert.Single(@event.EventAttendees, ea => ea.AttendeeId == attendeeId);
        Assert.Equal(ResponseStatus.Accepted, eventAttendee.ResponseStatus);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_WhenEventDoesNotExist_ThrowsNotFoundException()
    {
        var handler = new RecordAttendeeResponseCommandHandler(
            new FakeEventRepository(), new FakeDateTimeProvider(NowUtc), new FakeUnitOfWork());

        var command = new RecordAttendeeResponseCommand(Guid.NewGuid(), Guid.NewGuid(), ResponseStatus.Accepted, [1]);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
