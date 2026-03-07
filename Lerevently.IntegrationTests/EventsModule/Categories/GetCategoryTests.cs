using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Events.Application.Categories.GetCategory;
using Lerevently.Modules.Events.Domain.Categories;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.EventsModule.Categories;

public class GetCategoryTests : BaseIntegrationTest
{
    private IServiceScope _scope;
    private ISender Sender;

    [Before(Test)]
    public async Task SetupTest()
    {
        _scope = factory.Services.CreateScope();
        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
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
    public async Task Should_ReturnFailure_WhenCategoryDoesNotExist()
    {
        // Arrange
        var query = new GetCategoryQuery(Guid.NewGuid());

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Error.Should().Be(CategoryErrors.NotFound(query.CategoryId));
    }

    [Test]
    public async Task Should_ReturnCategory_WhenCategoryExists()
    {
        // Arrange
        Guid categoryId = await Sender.CreateCategoryAsync($"Category-{Guid.NewGuid()}");

        var query = new GetCategoryQuery(categoryId);

        // Act
        Result<CategoryResponse> result = await Sender.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }
}
