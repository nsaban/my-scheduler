using AutoMapper;
using MyScheduler.Application.Contracts;
using MyScheduler.Domain.Scheduling;

namespace MyScheduler.Application.Mapping;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<EventAttendee, EventAttendeeDto>();

        CreateMap<Event, EventSummaryDto>()
            .ForMember(d => d.StartTimeUtc, opt => opt.MapFrom(s => s.TimeRange.Start))
            .ForMember(d => d.EndTimeUtc, opt => opt.MapFrom(s => s.TimeRange.End));

        CreateMap<Event, EventDto>()
            .ForMember(d => d.StartTimeUtc, opt => opt.MapFrom(s => s.TimeRange.Start))
            .ForMember(d => d.EndTimeUtc, opt => opt.MapFrom(s => s.TimeRange.End))
            .ForMember(d => d.Attendees, opt => opt.MapFrom(s => s.EventAttendees))
            // RowVersion is an EF shadow property with no CLR member on Event; query-side code
            // that needs it (ETag responses) populates it directly via EF.Property, not AutoMapper.
            .ForMember(d => d.RowVersion, opt => opt.Ignore());
    }
}
