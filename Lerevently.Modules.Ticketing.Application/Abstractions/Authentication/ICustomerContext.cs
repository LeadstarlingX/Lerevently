namespace Lerevently.Modules.Ticketing.Application.Abstractions.Authentication;

public interface ICustomerContext
{
    Guid CustomerId { get; }
}