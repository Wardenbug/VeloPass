using VeloPass.Application.Abstractions;
using VeloPass.Application.Authentication;
using VeloPass.Domain.Abstractions;

namespace VeloPass.Application.Invites.ApplyInvite;

public sealed class AcceptInviteCommandHandler(
    IUserRegistrationService userRegistrationService)
{
    public async Task<Result<AccessTokenDto>> Handle(AcceptInviteCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var accessToken = await userRegistrationService.RegisterByInviteAsync(command.Token, cancellationToken);

        if (!accessToken.IsSuccess)
        {
            return Result.Invalid<AccessTokenDto>(accessToken.Error.Message);
        }

        return Result.Ok(accessToken.Value);
    }
}