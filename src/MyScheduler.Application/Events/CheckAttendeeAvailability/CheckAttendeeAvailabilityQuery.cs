using MediatR;

namespace MyScheduler.Application.Events.CheckAttendeeAvailability;

public sealed record CheckAttendeeAvailabilityQuery(Guid AttendeeId, DateTime StartTimeUtc, DateTime EndTimeUtc) : IRequest<bool>;
