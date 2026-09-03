using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyScheduler.Domain.Attendees;
using MyScheduler.Domain.ValueObjects;

namespace MyScheduler.Persistence.Configurations;

public sealed class AttendeeConfiguration : IEntityTypeConfiguration<Attendee>
{
    public void Configure(EntityTypeBuilder<Attendee> builder)
    {
        builder.ToTable("Attendees");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name)
            .HasMaxLength(Attendee.MaxNameLength)
            .IsRequired();

        builder.Property(a => a.Email)
            .HasConversion(email => email.Value, value => EmailAddress.Create(value))
            .HasMaxLength(EmailAddress.MaxLength)
            .IsRequired();

        builder.HasIndex(a => a.Email).IsUnique();

        builder.Property(a => a.CreatedAtUtc).IsRequired();
    }
}
