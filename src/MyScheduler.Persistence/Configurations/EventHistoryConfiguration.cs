using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyScheduler.Domain.Attendees;
using MyScheduler.Domain.History;
using MyScheduler.Domain.Scheduling;

namespace MyScheduler.Persistence.Configurations;

public sealed class EventHistoryConfiguration : IEntityTypeConfiguration<EventHistory>
{
    public void Configure(EntityTypeBuilder<EventHistory> builder)
    {
        builder.ToTable("EventHistory", tb =>
            tb.HasCheckConstraint("CK_EventHistory_Snapshot_IsJson", "ISJSON([Snapshot]) = 1"));

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Snapshot)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(h => h.ChangedAtUtc).IsRequired();

        builder.HasIndex(h => new { h.EventId, h.Version });

        builder.HasOne<Event>()
            .WithMany()
            .HasForeignKey(h => h.EventId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Attendee>()
            .WithMany()
            .HasForeignKey(h => h.ChangedByAttendeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
