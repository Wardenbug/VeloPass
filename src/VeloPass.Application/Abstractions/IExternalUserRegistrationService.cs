using VeloPass.Application.Authentication;
using VeloPass.Application.Identity;
using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Users;

namespace VeloPass.Application.Abstractions;

public interface IExternalUserRegistrationService
{
    Task<Result<AccessTokenDto>> RegisterAsync(ValidatedExternalIdentity externalIdentity, CancellationToken cancellationToken = default);
}