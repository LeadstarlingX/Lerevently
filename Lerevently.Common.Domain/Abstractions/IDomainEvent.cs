using MediatR;

namespace Lerevently.Common.Domain.Abstractions;

public interface IDomainEvent : INotification
{
    Guid Id { get; }
    DateTime OccurredAtUtc { get; }
}