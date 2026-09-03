using MyScheduler.Application.Common;
using MyScheduler.Application.Events.CreateEvent;
using MyScheduler.Application.UnitTests.Fakes;
using MyScheduler.Domain.Attendees;
using MyScheduler.Domain.ValueObjects;

namespace MyScheduler.Application.UnitTests.Events.CreateEvent;

public class CreateEventCommandHandlerTests
{
    private static readonly DateTime NowUtc = new(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task Handle_WhenValid_CreatesEventAndSaves()
    {
        var attendeeRepository = new FakeAttendeeRepository();
        var organizer = Attendee.Create("Dr. Smith", EmailAddress.Create("dr@practice.com"), NowUtc);
        attendeeRepository.Attendees[organizer.Id] = organizer;

        var eventRepository = new FakeEventRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new CreateEventCommandHandler(eventRepository, attendeeRepository, new FakeDateTimeProvider(NowUtc), unitOfWork);

        var command = new CreateEventCommand("Checkup", null, NowUtc, NowUtc.AddHours(1), organizer.Id, []);

        var eventId = await handler.Handle(command, CancellationToken.None);

        Assert.True(eventRepository.Events.ContainsKey(eventId));
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_WhenOrganizerNotFound_ThrowsNotFoundException()
    {
        var handler = new CreateEventCommandHandler(
            new FakeEventRepository(), new FakeAttendeeRepository(), new FakeDateTimeProvider(NowUtc), new FakeUnitOfWork());

        var command = new CreateEventCommand("Checkup", null, NowUtc, NowUtc.AddHours(1), Guid.NewGuid(), []);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenAttendeeIdMissing_ThrowsNotFoundException()
    {
        var attendeeRepository = new FakeAttendeeRepository();
        var organizer = Attendee.Create("Dr. Smith", EmailAddress.Create("dr@practice.com"), NowUtc);
        attendeeRepository.Attendees[organizer.Id] = organizer;

        var handler = new CreateEventCommandHandler(
            new FakeEventRepository(), attendeeRepository, new FakeDateTimeProvider(NowUtc), new FakeUnitOfWork());

        var command = new CreateEventCommand("Checkup", null, NowUtc, NowUtc.AddHours(1), organizer.Id, [Guid.NewGuid()]);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
