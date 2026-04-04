using VeloPass.Application.Abstractions;
using VeloPass.Application.Authentication;
using VeloPass.Application.Identity;
using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Users;

namespace VeloPass.Application.Users.LoginUser;

public sealed class LoginUserHandler(
    IExternalIdentityTokenValidator identityTokenValidator,
    IUserRepository userRepository,
    IJwtService jwtService)
{
    public async Task<Result<AccessTokenDto>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        
        ArgumentNullException.ThrowIfNull(command);
        
        var externalIdentityResult = await identityTokenValidator.ValidateAsync(
            ExternalIdentityProvider.Google,
            command.IdToken, cancellationToken);

        if (!externalIdentityResult.IsSuccess)
        {
            return Result.Invalid<AccessTokenDto>(externalIdentityResult.Error.Message);
        }

        var userResult = await userRepository.FindByEmailAsync(
            externalIdentityResult.Value.Email, cancellationToken);

        if (!userResult.IsSuccess)
        {
            return Result.NotFound<AccessTokenDto>("User not found");
        }

        var token = jwtService.GenerateJwtToken(new TokenRequest(userResult.Value.Id.ToString()));
        
        return Result.Ok(token);
    }
}