using VeloPass.Application.Abstractions;
using VeloPass.Application.Authentication;
using VeloPass.Domain.Abstractions;

namespace VeloPass.Application.Users.RefreshUser;

public sealed class RefreshUserHandler(
    IJwtService jwtService
    )
{
    public async Task<Result<AccessTokenDto>> Handle(
        RefreshUserCommand command, 
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var tokenResult = await jwtService.RefreshJwtToken(command.RefreshToken, cancellationToken);

        if (!tokenResult.IsSuccess)
        {
            return Result.Invalid<AccessTokenDto>("Invalid token");
        }
        
        return Result.Ok(tokenResult.Value);
    }
}