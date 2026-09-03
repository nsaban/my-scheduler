using MediatR;
using MyScheduler.Application.Abstractions;
using MyScheduler.Application.Common;
using MyScheduler.Domain.ValueObjects;

namespace MyScheduler.Application.Events.UpdateEvent;

public sealed class UpdateEventCommandHandler(
    IEventRepository eventRepository,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateEventCommand>
{
    public async Task Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        var @event = await eventRepository.GetForUpdateAsync(request.EventId, request.ExpectedVersion, cancellationToken)
            ?? throw new NotFoundException($"Event '{request.EventId}' was not found.");

        var timeRange = new DateTimeRange(request.StartTimeUtc, request.EndTimeUtc);

        @event.UpdateDetails(request.Title, request.Description, timeRange, dateTimeProvider.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
