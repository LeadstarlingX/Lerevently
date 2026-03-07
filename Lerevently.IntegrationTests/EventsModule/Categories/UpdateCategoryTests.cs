using FluentAssertions;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.IntegrationTests.Abstractions;
using Lerevently.Modules.Events.Application.Categories.UpdateCategory;
using Lerevently.Modules.Events.Domain.Categories;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.IntegrationTests.EventsModule.Categories;

public class UpdateCategoryTests : BaseIntegrationTest
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
    
    
    public static class TestDataSources
    {
        public static IEnumerable<Func<UpdateCategoryCommand>> AdditionTestData()
        {
            yield return () => new UpdateCategoryCommand(Guid.Empty, Faker.Music.Genre());
            yield return () => new UpdateCategoryCommand(Guid.NewGuid(), string.Empty);
            
        }
    }

    [Test]
    [MethodDataSource(typeof(TestDataSources), nameof(TestDataSources.AdditionTestData))]
    public async Task Should_ReturnFailure_WhenCommandIsNotValid(UpdateCategoryCommand command)
    {
        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Test]
    public async Task Should_ReturnFailure_WhenCategoryDoesNotExist()
    {
        // Arrange
        var command = new UpdateCategoryCommand(Guid.NewGuid(), Faker.Music.Genre());

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.Error.Should().Be(CategoryErrors.NotFound(command.CategoryId));
    }

    [Test]
    public async Task Should_UpdateCategory_WhenCategoryExists()
    {
        // Arrange
        Guid categoryId = await Sender.CreateCategoryAsync(Faker.Music.Genre());

        var command = new UpdateCategoryCommand(categoryId, Faker.Music.Genre());

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
