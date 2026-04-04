using VeloPass.Application.Abstractions;
using VeloPass.Domain.Abstractions;

namespace VeloPass.Application.Users.LogoutUser;

public sealed class LogoutUserHandler(
    IJwtService jwtService)
{
    public async Task<Result<bool>> Handle(
        LogoutUserCommand command, 
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var result = await jwtService.RevokeJwtToken(command.RefreshToken, cancellationToken);

        return result;
    }
}