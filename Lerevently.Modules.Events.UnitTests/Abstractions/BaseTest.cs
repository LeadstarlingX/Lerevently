using Bogus;
using Lerevently.Common.Domain.Abstractions;

namespace Lerevently.Modules.Events.UnitTests.Abstractions;

public abstract class BaseTest
{
    protected static readonly Faker Faker = new();
    
    public static T AssertDomainEventWasPublished<T>(Entity entity) where T : IDomainEvent
    {
        var domainEvent = entity.DomainEvents.OfType<T>().SingleOrDefault();

        if (domainEvent is null)
        {
            throw new Exception($"{typeof(T).Name} wasn't published.");
        }
        
        return domainEvent;
    }
}