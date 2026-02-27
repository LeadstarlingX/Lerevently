using System.Reflection;
using Lerevently.Modules.Events.Application;
using Lerevently.Modules.Events.Domain.Events;
using Lerevently.Modules.Events.Infrastructure;

namespace Lerevently.Modules.Events.ArchitectureTests.Abstractions;

public abstract class BaseTest
{
    protected static readonly Assembly ApplicationAssembly = typeof(AssemblyReference).Assembly;

    protected static readonly Assembly DomainAssembly = typeof(Event).Assembly;

    protected static readonly Assembly InfrastructureAssembly = typeof(EventsModule).Assembly;

    protected static readonly Assembly PresentationAssembly = typeof(Events.Presentation.AssemblyReference).Assembly;
}