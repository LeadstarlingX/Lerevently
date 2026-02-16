using Lerevently.Common.Domain.Abstractions;

namespace Lerevently.Modules.Events.Domain.Categories;

public static class CategoryErrors
{
    public static readonly Error AlreadyArchived = Error.Problem(
        "Categories.AlreadyArchived",
        "The category was already archived");

    public static Error NotFound(Guid categoryId)
    {
        return Error.NotFound("Categories.NotFound", $"The category with the identifier {categoryId} was not found");
    }
}