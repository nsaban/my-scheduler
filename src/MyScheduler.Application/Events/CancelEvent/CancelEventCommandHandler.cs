using MediatR;
using MyScheduler.Application.Abstractions;
using MyScheduler.Application.Common;

namespace MyScheduler.Application.Events.CancelEvent;

public sealed class CancelEventCommandHandler(
    IEventRepository eventRepository,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork) : IRequestHandler<CancelEventCommand>
{
    public async Task Handle(CancelEventCommand request, CancellationToken cancellationToken)
    {
        var @event = await eventRepository.GetForUpdateAsync(request.EventId, request.ExpectedVersion, cancellationToken)
            ?? throw new NotFoundException($"Event '{request.EventId}' was not found.");

        @event.Cancel(dateTimeProvider.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
