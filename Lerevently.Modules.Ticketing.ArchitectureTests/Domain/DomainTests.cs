using System.Reflection;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Ticketing.ArchitectureTests.Abstractions;
using NetArchTest.Rules;

namespace Lerevently.Modules.Ticketing.ArchitectureTests.Domain;

public class DomainTests : BaseTest
{
    [Test]
    public async Task DomainEvents_Should_BeSealed()
    {
        var result = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(DomainEvent))
            .Should()
            .BeSealed()
            .GetResult();
        
        await Assert.That(result.IsSuccessful).IsTrue();
    }

    [Test]
    public async Task DomainEvent_ShouldHave_DomainEventPostfix()
    {
        var result = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(DomainEvent))
            .Should()
            .HaveNameEndingWith("DomainEvent")
            .GetResult();
        
        await Assert.That(result.IsSuccessful).IsTrue();
    }

    [Test]
    public async Task Entities_ShouldHave_PrivateParameterlessConstructor()
    {
        IEnumerable<Type> entityTypes =  Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(Entity))
            .GetTypes();

        var failingTypes = new List<Type>();
        foreach (Type entityType in entityTypes)
        {
            ConstructorInfo[] constructors = entityType.GetConstructors(BindingFlags.NonPublic |
                                                                        BindingFlags.Instance);

            if (!constructors.Any(c => c.IsPrivate && c.GetParameters().Length == 0))
            {
                failingTypes.Add(entityType);
            }
        }

        await Assert.That(!failingTypes.Any()).IsTrue();
    }

    [Test]
    public async Task Entities_ShouldOnlyHave_PrivateConstructors()
    {
        IEnumerable<Type> entityTypes = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(Entity))
            .GetTypes();

        var failingTypes = new List<Type>();
        foreach (Type entityType in entityTypes)
        {
            ConstructorInfo[] constructors = entityType.GetConstructors(BindingFlags.Public |
                                                                        BindingFlags.Instance);

            if (constructors.Any())
            {
                failingTypes.Add(entityType);
            }
        }

        await Assert.That(!failingTypes.Any()).IsTrue();
    }
}
