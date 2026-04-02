using VeloPass.Application.Abstractions;
using VeloPass.Application.Authentication;

namespace VeloPass.Application.Users.RefreshUser;

public sealed class RefreshUserHandler(
    IJwtService jwtService
    )
{
    public async Task<AccessTokenDto> Handle(
        RefreshUserCommand command, 
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var token = await jwtService.RefreshJwtToken(command.RefreshToken, cancellationToken);

        return token;
    }
}