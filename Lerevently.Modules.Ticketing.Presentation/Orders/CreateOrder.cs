using Lerevently.Common.Presentation.ApiResults;
using Lerevently.Common.Presentation.Endpoints;
using Lerevently.Modules.Ticketing.Application.Abstractions.Authentication;
using Lerevently.Modules.Ticketing.Application.Orders.CreateOrder;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Lerevently.Modules.Ticketing.Presentation.Orders;

internal sealed class CreateOrder : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("orders", async (ICustomerContext customerContext, ISender sender) =>
            {
                var result = await sender.Send(new CreateOrderCommand(customerContext.CustomerId));

                return result.Match(() => Results.Ok(), ApiResults.Problem);
            })
            .RequireAuthorization(Permissions.CreateOrder)
            .WithTags(Tags.Orders);
    }
}