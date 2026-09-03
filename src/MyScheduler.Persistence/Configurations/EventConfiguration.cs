using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyScheduler.Domain.Attendees;
using MyScheduler.Domain.Scheduling;

namespace MyScheduler.Persistence.Configurations;

public sealed class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("Events", tb =>
            tb.HasCheckConstraint("CK_Event_EndAfterStart", "[EndTimeUtc] > [StartTimeUtc]"));

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .HasMaxLength(Event.MaxTitleLength)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(Event.MaxDescriptionLength);

        builder.OwnsOne(e => e.TimeRange, timeRange =>
        {
            timeRange.Property(r => r.Start).HasColumnName("StartTimeUtc").IsRequired();
            timeRange.Property(r => r.End).HasColumnName("EndTimeUtc").IsRequired();
            timeRange.HasIndex(r => new { r.Start, r.End });
        });
        builder.Navigation(e => e.TimeRange).IsRequired();

        builder.HasIndex(e => e.Status);

        builder.Property(e => e.CreatedAtUtc).IsRequired();
        builder.Property(e => e.UpdatedAtUtc).IsRequired();

        // SQL Server's native optimistic-concurrency token; not a Domain property, so Domain stays persistence-ignorant.
        builder.Property<byte[]>("RowVersion").IsRowVersion();

        builder.HasOne<Attendee>()
            .WithMany()
            .HasForeignKey(e => e.OrganizerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.EventAttendees)
            .WithOne()
            .HasForeignKey(ea => ea.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(e => e.EventAttendees)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
