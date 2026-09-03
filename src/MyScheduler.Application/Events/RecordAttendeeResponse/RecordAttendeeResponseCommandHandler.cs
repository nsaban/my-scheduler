using MediatR;
using MyScheduler.Application.Abstractions;
using MyScheduler.Application.Common;

namespace MyScheduler.Application.Events.RecordAttendeeResponse;

public sealed class RecordAttendeeResponseCommandHandler(
    IEventRepository eventRepository,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork) : IRequestHandler<RecordAttendeeResponseCommand>
{
    public async Task Handle(RecordAttendeeResponseCommand request, CancellationToken cancellationToken)
    {
        var @event = await eventRepository.GetForUpdateAsync(request.EventId, request.ExpectedVersion, cancellationToken)
            ?? throw new NotFoundException($"Event '{request.EventId}' was not found.");

        @event.RecordAttendeeResponse(request.AttendeeId, request.Response, dateTimeProvider.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
