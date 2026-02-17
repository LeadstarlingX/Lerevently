using Microsoft.AspNetCore.Routing;

namespace Lerevently.Common.Presentation.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}