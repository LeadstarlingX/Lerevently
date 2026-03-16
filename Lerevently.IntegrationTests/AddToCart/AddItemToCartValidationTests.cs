using FluentAssertions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Ticketing.Application.Carts.AddItemToCart;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.AddToCart;

public sealed class AddItemToCartValidationTests : BaseIntegrationTest
{
    private IServiceScope _scope;
    private ISender _sender;

    [Before(Test)]
    public async Task Setup()
    {
        _scope = factory.Services.CreateScope();
        _sender = _scope.ServiceProvider.GetRequiredService<ISender>();
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
    public async Task Should_ReturnError_WhenCustomerDoesNotExist()
    {
        // Arrange
        var command = new AddItemToCartCommand(Guid.NewGuid(), Guid.NewGuid(), 1);

        // Act
        var result = await _sender.Send(command);

        // Assert
        result.IsFailure.Should().BeTrue();
    }
}