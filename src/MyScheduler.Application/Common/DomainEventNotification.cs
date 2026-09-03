using MediatR;
using MyScheduler.Domain.Common;

namespace MyScheduler.Application.Common;

public sealed record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent) : INotification
    where TDomainEvent : IDomainEvent;
