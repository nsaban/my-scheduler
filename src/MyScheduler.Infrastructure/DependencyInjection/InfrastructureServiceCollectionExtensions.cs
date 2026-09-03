using Microsoft.Extensions.DependencyInjection;
using MyScheduler.Application.Abstractions;
using MyScheduler.Infrastructure.Notifications;
using MyScheduler.Infrastructure.Outbox;
using MyScheduler.Infrastructure.Time;

namespace MyScheduler.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        services.AddTransient<INotificationChannelSender, EmailNotificationSender>();
        services.AddTransient<INotificationChannelSender, IcalNotificationSender>();
        services.AddTransient<INotificationChannelSender, MqNotificationSender>();

        services.AddScoped<NotificationDispatcher>();
        services.AddHostedService<NotificationDispatcherBackgroundService>();

        return services;
    }
}
