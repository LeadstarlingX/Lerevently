using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Attendance.Infrastructure.Database;
using Lerevently.Modules.Events.Infrastructure.Database;
using Lerevently.Modules.Ticketing.Application.Carts.AddItemToCart;
using Lerevently.Modules.Ticketing.Application.Customers.GetCustomer;
using Lerevently.Modules.Ticketing.Infrastructure.Database;
using Lerevently.Modules.Users.Application.Users.RegisterUser;
using Lerevently.Modules.Users.Infrastructure.Database;
using Lerevently.Modules.Users.Infrastructure.Identity;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Lerevently.IntegrationTests.AddToCart;

public sealed class AddItemToCartTests : BaseIntegrationTest
{
    private const decimal Quantity = 10;
    
    private IServiceScope _scope;
    private static KeyCloakOptions _options;
    private static UsersDbContext Users_DbContext;
    private static EventsDbContext Events_DbContext;
    private static TicketingDbContext Ticketing_DbContext;
    private static AttendanceDbContext Attendance_DbContext;
    
    [Before(Test)]
    public async Task SetupTest()
    {
        _scope = factory.Services.CreateScope();
        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        _options = _scope.ServiceProvider.GetRequiredService<IOptions<KeyCloakOptions>>().Value;
        Users_DbContext = _scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        Ticketing_DbContext = _scope.ServiceProvider.GetRequiredService<TicketingDbContext>();

        // Fresh DbContext, clean state
        Users_DbContext.Users.RemoveRange(Users_DbContext.Users);
        Ticketing_DbContext.Customers.RemoveRange(Ticketing_DbContext.Customers);
        await DbContext.SaveChangesAsync();
    }
    
    
    [After(Test)]
    public async ValueTask TeardownTest()
    {
        if (_scope is IAsyncDisposable asyncScope)
            await asyncScope.DisposeAsync();
        else
            _scope.Dispose();
    }
    
    

    [Test]
    public async Task Customer_ShouldBeAbleTo_AddItemToCart()
    {
        // Register user
        var command = new RegisterUserCommand(
            Faker.Internet.Email(),
            Faker.Internet.Password(6),
            Faker.Name.FirstName(),
            Faker.Name.LastName());

        Result<Guid> userResult = await Sender.Send(command);

        userResult.IsSuccess.Should().BeTrue();

        // Get customer
        Result<CustomerResponse> customerResult = await Poller.WaitAsync(
            TimeSpan.FromSeconds(15),
            async () =>
            {
                var query = new GetCustomerQuery(userResult.Value);

                Result<CustomerResponse> customerResult = await Sender.Send(query);

                return customerResult;
            });

        customerResult.IsSuccess.Should().BeTrue();

        // Add item to cart
        CustomerResponse customer = customerResult.Value;
        var ticketTypeId = Guid.NewGuid();

        await Sender.CreateEventAsync(Guid.NewGuid(), ticketTypeId, Quantity);

        Result result = await Sender.Send(new AddItemToCartCommand(customer.Id, ticketTypeId, Quantity));

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
