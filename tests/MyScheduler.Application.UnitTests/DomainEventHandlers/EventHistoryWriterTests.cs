using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using MyScheduler.Application.DomainEventHandlers;
using MyScheduler.Application.Mapping;
using MyScheduler.Application.UnitTests.Fakes;
using MyScheduler.Domain.History;
using MyScheduler.Domain.Scheduling;
using MyScheduler.Domain.ValueObjects;

namespace MyScheduler.Application.UnitTests.DomainEventHandlers;

public class EventHistoryWriterTests
{
    private static readonly DateTime NowUtc = new(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);

    private static IMapper CreateMapper()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>(), NullLoggerFactory.Instance);
        return configuration.CreateMapper();
    }

    [Fact]
    public async Task RecordAsync_WhenCreated_RecordsHistoryWithOrganizerAsActor()
    {
        var organizerId = Guid.NewGuid();
        var @event = Event.Schedule("Checkup", "Notes", new DateTimeRange(NowUtc, NowUtc.AddHours(1)), organizerId, [], NowUtc);

        var eventRepository = new FakeEventRepository();
        eventRepository.Events[@event.Id] = @event;
        var historyRepository = new FakeEventHistoryRepository();

        var writer = new EventHistoryWriter(eventRepository, historyRepository, CreateMapper());

        await writer.RecordAsync(@event.Id, EventChangeType.Created, NowUtc, CancellationToken.None);

        var record = Assert.Single(historyRepository.Records);
        Assert.Equal(EventChangeType.Created, record.ChangeType);
        Assert.Equal(1, record.Version);
        Assert.Equal(organizerId, record.ChangedByAttendeeId);
        Assert.Contains("Checkup", record.Snapshot);
    }

    [Fact]
    public async Task RecordAsync_WhenUpdated_ActorIsUnknown()
    {
        var @event = Event.Schedule("Checkup", null, new DateTimeRange(NowUtc, NowUtc.AddHours(1)), Guid.NewGuid(), [], NowUtc);

        var eventRepository = new FakeEventRepository();
        eventRepository.Events[@event.Id] = @event;
        var historyRepository = new FakeEventHistoryRepository();

        var writer = new EventHistoryWriter(eventRepository, historyRepository, CreateMapper());

        await writer.RecordAsync(@event.Id, EventChangeType.Updated, NowUtc, CancellationToken.None);

        var record = Assert.Single(historyRepository.Records);
        Assert.Null(record.ChangedByAttendeeId);
    }

    [Fact]
    public async Task RecordAsync_AssignsIncrementingVersionPerEvent()
    {
        var @event = Event.Schedule("Checkup", null, new DateTimeRange(NowUtc, NowUtc.AddHours(1)), Guid.NewGuid(), [], NowUtc);

        var eventRepository = new FakeEventRepository();
        eventRepository.Events[@event.Id] = @event;
        var historyRepository = new FakeEventHistoryRepository();

        var writer = new EventHistoryWriter(eventRepository, historyRepository, CreateMapper());

        await writer.RecordAsync(@event.Id, EventChangeType.Created, NowUtc, CancellationToken.None);
        await writer.RecordAsync(@event.Id, EventChangeType.Updated, NowUtc.AddMinutes(1), CancellationToken.None);

        Assert.Equal([1, 2], historyRepository.Records.Select(r => r.Version));
    }
}
