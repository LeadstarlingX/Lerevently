namespace Lerevently.ArchitectureTests.Abstractions;

public abstract class BaseTest
{
    protected const string UsersNamespace = "Lerevently.Modules.Users";
    protected const string UsersIntegrationEventsNamespace = "Lerevently.Modules.Users.IntegrationEvents";

    protected const string EventsNamespace = "Lerevently.Modules.Events";
    protected const string EventsIntegrationEventsNamespace = "Lerevently.Modules.Events.IntegrationEvents";

    protected const string TicketingNamespace = "Lerevently.Modules.Ticketing";
    protected const string TicketingIntegrationEventsNamespace = "Lerevently.Modules.Ticketing.IntegrationEvents";

    protected const string AttendanceNamespace = "Lerevently.Modules.Attendance";
    protected const string AttendanceIntegrationEventsNamespace = "Lerevently.Modules.Attendance.IntegrationEvents";
}