using MediatR;
using MyScheduler.Application.Abstractions;
using MyScheduler.Application.Common;
using MyScheduler.Domain.Scheduling;
using MyScheduler.Domain.ValueObjects;

namespace MyScheduler.Application.Events.CreateEvent;

public sealed class CreateEventCommandHandler(
    IEventRepository eventRepository,
    IAttendeeRepository attendeeRepository,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateEventCommand, Guid>
{
    public async Task<Guid> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        var organizer = await attendeeRepository.GetByIdAsync(request.OrganizerId, cancellationToken)
            ?? throw new NotFoundException($"Attendee '{request.OrganizerId}' was not found.");

        if (request.AttendeeIds.Count > 0)
        {
            var attendees = await attendeeRepository.GetByIdsAsync(request.AttendeeIds, cancellationToken);
            var missingIds = request.AttendeeIds.Except(attendees.Select(a => a.Id)).ToList();
            if (missingIds.Count > 0)
            {
                throw new NotFoundException($"Attendee(s) not found: {string.Join(", ", missingIds)}.");
            }
        }

        var timeRange = new DateTimeRange(request.StartTimeUtc, request.EndTimeUtc);

        var @event = Event.Schedule(
            request.Title,
            request.Description,
            timeRange,
            organizer.Id,
            request.AttendeeIds,
            dateTimeProvider.UtcNow);

        await eventRepository.AddAsync(@event, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return @event.Id;
    }
}
