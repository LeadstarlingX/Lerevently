namespace Lerevently.Modules.Events.Domain.Abstractions;

public abstract class DomainEvent : IDomainEvent
{
    protected DomainEvent()
    {
        Id = Guid.NewGuid();
        OccurredAtUtc = DateTime.UtcNow;
    }

    protected DomainEvent(Guid id, DateTime occurredAtUtc)
    {
        Id = id;
        OccurredAtUtc = occurredAtUtc;
    }

    public Guid Id { get; init; }
    public DateTime OccurredAtUtc { get; init; }
}