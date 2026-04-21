using VeloPass.Application.Authentication;
using VeloPass.Application.Identity;
using VeloPass.Domain.Abstractions;

namespace VeloPass.Application.Abstractions;

public interface IUserRegistrationService
{
    Task<Result<AccessTokenDto>> RegisterByExternalProviderAsync(ValidatedExternalIdentity externalIdentity,
        CancellationToken cancellationToken = default);
    
    Task<Result<AccessTokenDto>> RegisterByInviteAsync(string token, CancellationToken cancellationToken = default);
}