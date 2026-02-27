using Lerevently.Common.Domain.Abstractions;

namespace Lerevently.Modules.Ticketing.Domain.Customers;

public static class CustomerErrors
{
    public static Error NotFound(Guid customerId)
    {
        return Error.NotFound("Customers.NotFound", $"The customer with the identifier {customerId} was not found");
    }
}