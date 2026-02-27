using Lerevently.Modules.Events.ArchitectureTests.Abstractions;
using MassTransit;
using NetArchTest.Rules;

namespace Lerevently.Modules.Events.ArchitectureTests.Presentation;

public class PresentationTests : BaseTest
{
    [Test]
    public async Task IntegrationEventHandler_Should_BeSealed()
    {
        var result = Types.InAssembly(PresentationAssembly)
            .That()
            .ImplementInterface(typeof(IConsumer<>))
            .Should()
            .BeSealed()
            .GetResult();

        await Assert.That(result.IsSuccessful).IsTrue();
    }

    [Test]
    public async Task IntegrationEventHandler_ShouldHave_NameEndingWith_DomainEventHandler()
    {
        var result = Types.InAssembly(PresentationAssembly)
            .That()
            .ImplementInterface(typeof(IConsumer<>))
            .Should()
            .HaveNameEndingWith("Consumer")
            .GetResult();

        await Assert.That(result.IsSuccessful).IsTrue();
    }
}