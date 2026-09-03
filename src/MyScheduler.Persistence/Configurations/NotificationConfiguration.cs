using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyScheduler.Domain.Attendees;
using MyScheduler.Domain.Notifications;
using MyScheduler.Domain.Scheduling;

namespace MyScheduler.Persistence.Configurations;

public sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications", tb =>
            tb.HasCheckConstraint("CK_Notification_Payload_IsJson", "ISJSON([Payload]) = 1"));

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Payload)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(n => n.LastErrorMessage)
            .HasMaxLength(Notification.MaxLastErrorMessageLength);

        builder.Property(n => n.CreatedAtUtc).IsRequired();

        builder.HasIndex(n => n.Status);

        builder.HasOne<Event>()
            .WithMany()
            .HasForeignKey(n => n.EventId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Attendee>()
            .WithMany()
            .HasForeignKey(n => n.RecipientAttendeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
