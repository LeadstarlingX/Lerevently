using Lerevently.Common.Domain.Abstractions;
using MediatR;

namespace Lerevently.Common.Application.Messaging;

public interface IDomainEventHandler<in TDomainEvent> : INotificationHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent;
