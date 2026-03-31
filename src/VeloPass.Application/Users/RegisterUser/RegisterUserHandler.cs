using VeloPass.Application.Abstractions;
using VeloPass.Application.Authentication;
using VeloPass.Application.Identity;
using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Users;

namespace VeloPass.Application.Users.RegisterUser;

public sealed class RegisterUserHandler(
    IExternalIdentityTokenValidator identityTokenValidator,
    IExternalUserRegistrationService externalUserRegistrationService,
    IJwtService jwtService)
{
    
    public async Task<AccessTokenDto> Handle(RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var providerResult = await identityTokenValidator.ValidateAsync(
            ExternalIdentityProvider.Google,
            command.IdToken, 
            cancellationToken);
        
        var user = await externalUserRegistrationService.RegisterAsync(providerResult, cancellationToken);

        var token = jwtService.GenerateJwtToken(new TokenRequest(user.Id.ToString()));

        return token;
    }
}