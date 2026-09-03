using System.Text.Json;
using AutoMapper;
using MyScheduler.Application.Abstractions;
using MyScheduler.Application.Contracts;
using MyScheduler.Domain.History;

namespace MyScheduler.Application.DomainEventHandlers;

public sealed class EventHistoryWriter(
    IEventRepository eventRepository,
    IEventHistoryRepository eventHistoryRepository,
    IMapper mapper)
{
    public async Task RecordAsync(
        Guid eventId,
        EventChangeType changeType,
        DateTime occurredOnUtc,
        CancellationToken cancellationToken)
    {
        var @event = await eventRepository.GetByIdAsync(eventId, cancellationToken)
            ?? throw new InvalidOperationException($"Event '{eventId}' was not found while recording history.");

        var snapshot = mapper.Map<EventDto>(@event);
        var snapshotJson = JsonSerializer.Serialize(snapshot);
        var version = await eventHistoryRepository.GetNextVersionAsync(eventId, cancellationToken);

        // Only the Created case has a known actor (the organizer); Updated/Cancelled don't carry
        // one because no authentication/actor context exists in this system yet.
        Guid? changedByAttendeeId = changeType == EventChangeType.Created ? @event.OrganizerId : null;

        var history = EventHistory.Record(eventId, version, changeType, snapshotJson, changedByAttendeeId, occurredOnUtc);

        await eventHistoryRepository.AddAsync(history, cancellationToken);
    }
}
