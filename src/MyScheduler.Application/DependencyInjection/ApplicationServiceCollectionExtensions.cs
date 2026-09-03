using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MyScheduler.Application.Behaviors;
using MyScheduler.Application.Common;
using MyScheduler.Application.DomainEventHandlers;
using MyScheduler.Domain.Scheduling.DomainEvents;

namespace MyScheduler.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // .NET DI's open-generic support doesn't close over a type parameter that's nested inside
        // the service's generic argument (INotificationHandler<DomainEventNotification<T>>), so each
        // of our (small, known) domain event types needs its own explicit closed registration.
        services.AddTransient<INotificationHandler<DomainEventNotification<EventCreatedDomainEvent>>, DomainEventNotificationHandler<EventCreatedDomainEvent>>();
        services.AddTransient<INotificationHandler<DomainEventNotification<EventUpdatedDomainEvent>>, DomainEventNotificationHandler<EventUpdatedDomainEvent>>();
        services.AddTransient<INotificationHandler<DomainEventNotification<EventCancelledDomainEvent>>, DomainEventNotificationHandler<EventCancelledDomainEvent>>();
        services.AddTransient<INotificationHandler<DomainEventNotification<AttendeeResponseRecordedDomainEvent>>, DomainEventNotificationHandler<AttendeeResponseRecordedDomainEvent>>();

        services.AddValidatorsFromAssembly(assembly);

        services.AddAutoMapper(cfg => cfg.AddMaps(assembly));

        services.AddTransient<EventHistoryWriter>();
        services.AddTransient<AttendeeNotificationWriter>();

        services.AddTransient<IDomainEventHandler<EventCreatedDomainEvent>, AppendEventHistoryOnEventCreated>();
        services.AddTransient<IDomainEventHandler<EventCreatedDomainEvent>, NotifyAttendeesOnEventCreated>();
        services.AddTransient<IDomainEventHandler<EventUpdatedDomainEvent>, AppendEventHistoryOnEventUpdated>();
        services.AddTransient<IDomainEventHandler<EventUpdatedDomainEvent>, NotifyAttendeesOnEventUpdated>();
        services.AddTransient<IDomainEventHandler<EventCancelledDomainEvent>, AppendEventHistoryOnEventCancelled>();
        services.AddTransient<IDomainEventHandler<EventCancelledDomainEvent>, NotifyAttendeesOnEventCancelled>();
        services.AddTransient<IDomainEventHandler<AttendeeResponseRecordedDomainEvent>, NotifyOrganizerOnAttendeeResponseRecorded>();

        return services;
    }
}
