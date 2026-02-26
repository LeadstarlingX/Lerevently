using Lerevently.Common.Domain.Abstractions;

namespace Lerevently.Modules.Users.Application.Abstractions.Identity;

public interface IIdentityProviderService
{
    Task<Result<string>> RegisterUserAsync(UserModel user, CancellationToken cancellationToken = default);
    
    Task<Result<bool>> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default);
}
