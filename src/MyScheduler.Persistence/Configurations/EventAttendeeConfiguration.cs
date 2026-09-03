using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyScheduler.Domain.Attendees;
using MyScheduler.Domain.Scheduling;

namespace MyScheduler.Persistence.Configurations;

public sealed class EventAttendeeConfiguration : IEntityTypeConfiguration<EventAttendee>
{
    public void Configure(EntityTypeBuilder<EventAttendee> builder)
    {
        builder.ToTable("EventAttendees");

        builder.HasKey(ea => new { ea.EventId, ea.AttendeeId });

        builder.HasIndex(ea => ea.AttendeeId);

        builder.HasOne<Attendee>()
            .WithMany()
            .HasForeignKey(ea => ea.AttendeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
