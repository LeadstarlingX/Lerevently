using Lerevently.Common.Application.Messaging;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Users.Application.Abstractions.Data;
using Lerevently.Modules.Users.Application.Abstractions.Identity;
using Lerevently.Modules.Users.Domain.Users;

namespace Lerevently.Modules.Users.Application.Users.RegisterUser;

internal sealed class RegisterUserCommandHandler(IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IIdentityProviderService identityProviderService)
    : ICommandHandler<RegisterUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var result = await identityProviderService.RegisterUserAsync(new UserModel(
            request.Email, request.Password,request.FirstName, request.LastName), cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }
        
        var user = User.Create(request.Email, request.FirstName, request.LastName, result.Value);

        userRepository.Insert(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);


        return user.Id;
    }
}
