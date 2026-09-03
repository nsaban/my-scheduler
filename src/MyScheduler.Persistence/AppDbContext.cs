using Microsoft.EntityFrameworkCore;
using MyScheduler.Domain.Attendees;
using MyScheduler.Domain.History;
using MyScheduler.Domain.Notifications;
using MyScheduler.Domain.Scheduling;

namespace MyScheduler.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Event> Events => Set<Event>();

    public DbSet<Attendee> Attendees => Set<Attendee>();

    public DbSet<EventHistory> EventHistory => Set<EventHistory>();

    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
