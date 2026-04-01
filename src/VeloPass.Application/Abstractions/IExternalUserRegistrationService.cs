using VeloPass.Application.Authentication;
using VeloPass.Application.Identity;
using VeloPass.Domain.Users;

namespace VeloPass.Application.Abstractions;

public interface IExternalUserRegistrationService
{
    Task<AccessTokenDto> RegisterAsync(ValidatedExternalIdentity externalIdentity, CancellationToken cancellationToken = default);
}