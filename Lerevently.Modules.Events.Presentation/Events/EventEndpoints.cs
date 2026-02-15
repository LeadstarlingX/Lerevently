using Microsoft.AspNetCore.Routing;

namespace Lerevently.Modules.Events.Presentation.Events;

public static class EventEndpoints
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        CreateEvent.MapEndpoint(app);
        GetEvent.MapEndpoint(app);
    }
}