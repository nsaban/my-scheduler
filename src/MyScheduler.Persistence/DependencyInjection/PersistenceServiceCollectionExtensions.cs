using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyScheduler.Application.Abstractions;
using MyScheduler.Persistence.Outbox;
using MyScheduler.Persistence.Queries;
using MyScheduler.Persistence.Repositories;

namespace MyScheduler.Persistence.DependencyInjection;

public static class PersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IAttendeeRepository, AttendeeRepository>();
        services.AddScoped<IEventHistoryRepository, EventHistoryRepository>();
        services.AddScoped<INotificationOutboxRepository, NotificationOutboxRepository>();
        services.AddScoped<IEventListingQueries, EventListingQueries>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
