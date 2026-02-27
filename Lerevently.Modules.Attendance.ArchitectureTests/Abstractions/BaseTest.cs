using System.Reflection;
using Lerevently.Modules.Attendance.Application;
using Lerevently.Modules.Attendance.Domain.Events;
using Lerevently.Modules.Attendance.Infrastructure;

namespace Lerevently.Modules.Attendance.ArchitectureTests.Abstractions;

public abstract class BaseTest
{
    protected static readonly Assembly ApplicationAssembly = typeof(AssemblyReference).Assembly;

    protected static readonly Assembly DomainAssembly = typeof(Event).Assembly;

    protected static readonly Assembly InfrastructureAssembly = typeof(AttendanceModule).Assembly;

    protected static readonly Assembly
        PresentationAssembly = typeof(Attendance.Presentation.AssemblyReference).Assembly;
}