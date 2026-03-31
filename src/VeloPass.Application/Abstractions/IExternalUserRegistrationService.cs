using VeloPass.Application.Identity;
using VeloPass.Domain.Users;

namespace VeloPass.Application.Abstractions;

public interface IExternalUserRegistrationService
{
    Task<User> RegisterAsync(ValidatedExternalIdentity externalIdentity, CancellationToken cancellationToken = default);
}