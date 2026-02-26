using System.Reflection;
using Lerevently.ArchitectureTests.Abstractions;
using Lerevently.Modules.Attendance.Domain.Attendees;
using Lerevently.Modules.Attendance.Infrastructure;
using Lerevently.Modules.Events.Domain.Events;
using Lerevently.Modules.Events.Infrastructure;
using Lerevently.Modules.Ticketing.Domain.Orders;
using Lerevently.Modules.Ticketing.Infrastructure;
using Lerevently.Modules.Users.Domain.Users;
using Lerevently.Modules.Users.Infrastructure;
using NetArchTest.Rules;

namespace Lerevently.ArchitectureTests;

public class ModulesTests : BaseTest
{
    [Test]
    public async Task UsersModule_ShouldNotHaveDependencyOn_AnyOtherModule()
    {
        string[] otherModules = [EventsNamespace, TicketingNamespace, AttendanceNamespace];
        string[] integrationEventsModules =
        [
            EventsIntegrationEventsNamespace,
            TicketingIntegrationEventsNamespace,
            AttendanceIntegrationEventsNamespace
        ];

        List<Assembly> usersAssemblies =
        [
            typeof(User).Assembly,
            Modules.Users.Application.AssemblyReference.Assembly,
            Modules.Users.Presentation.AssemblyReference.Assembly,
            typeof(UsersModule).Assembly
        ];

        var result = Types.InAssemblies(usersAssemblies)
            .That()
            .DoNotHaveDependencyOnAny(integrationEventsModules)
            .Should()
            .NotHaveDependencyOnAny(otherModules)
            .GetResult();

        await Assert.That(result.IsSuccessful).IsTrue();
    }
    
    
    [Test]
    public async Task EventsModule_ShouldNotHaveDependencyOn_AnyOtherModule()
    {
        string[] otherModules = [UsersNamespace, TicketingNamespace, AttendanceNamespace];
        string[] integrationEventsModules =
        [
            UsersIntegrationEventsNamespace,
            TicketingIntegrationEventsNamespace,
            AttendanceIntegrationEventsNamespace
        ];

        List<Assembly> eventsAssemblies =
        [
            typeof(Event).Assembly,
            Modules.Events.Application.AssemblyReference.Assembly,
            Modules.Events.Presentation.AssemblyReference.Assembly,
            typeof(EventsModule).Assembly
        ];

        var result = Types.InAssemblies(eventsAssemblies)
            .That()
            .DoNotHaveDependencyOnAny(integrationEventsModules)
            .Should()
            .NotHaveDependencyOnAny(otherModules)
            .GetResult();

        await Assert.That(result.IsSuccessful).IsTrue();
    }
    
    
    [Test]
    public async Task TicketingModule_ShouldNotHaveDependencyOn_AnyOtherModule()
    {
        string[] otherModules = [EventsNamespace, UsersNamespace, AttendanceNamespace];
        string[] integrationEventsModules =
        [
            EventsIntegrationEventsNamespace,
            UsersIntegrationEventsNamespace,
            AttendanceIntegrationEventsNamespace
        ];

        List<Assembly> ticketingAssemblies =
        [
            typeof(Order).Assembly,
            Modules.Ticketing.Application.AssemblyReference.Assembly,
            Modules.Ticketing.Presentation.AssemblyReference.Assembly,
            typeof(TicketingModule).Assembly
        ];

        var result = Types.InAssemblies(ticketingAssemblies)
            .That()
            .DoNotHaveDependencyOnAny(integrationEventsModules)
            .Should()
            .NotHaveDependencyOnAny(otherModules)
            .GetResult();

        await Assert.That(result.IsSuccessful).IsTrue();
    }
    
    
    [Test]
    public async Task AttendanceModule_ShouldNotHaveDependencyOn_AnyOtherModule()
    {
        string[] otherModules = [UsersNamespace, TicketingNamespace, EventsNamespace];
        string[] integrationEventsModules =
        [
            UsersIntegrationEventsNamespace,
            TicketingIntegrationEventsNamespace,
            EventsIntegrationEventsNamespace
        ];

        List<Assembly> attendanceAssemblies =
        [
            typeof(Attendee).Assembly,
            Modules.Attendance.Application.AssemblyReference.Assembly,
            Modules.Attendance.Presentation.AssemblyReference.Assembly,
            typeof(AttendanceModule).Assembly
        ];
        

        var result = Types.InAssemblies(attendanceAssemblies)
            .That()
            .DoNotHaveDependencyOnAny(integrationEventsModules)
            .Should()
            .NotHaveDependencyOnAny(otherModules)
            .GetResult();

        await Assert.That(result.IsSuccessful).IsTrue();
    }
    
}
