using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Ticketing.Domain.Customers;
using Lerevently.Modules.Ticketing.UnitTests.Abstractions;

namespace Lerevently.Modules.Ticketing.UnitTests.Customers;

public class CustomerTests : BaseTest
{
    [Test]
    public async Task Create_ShouldReturnValue_WhenCustomerIsCreated()
    {
        //Act
        Result<Customer> result = Customer.Create(
            Guid.NewGuid(), 
            Faker.Internet.Email(),
            Faker.Name.FirstName(),
            Faker.Name.LastName());
        //Assert
        await Assert.That(result.Value).IsNotNull();
    }
}
