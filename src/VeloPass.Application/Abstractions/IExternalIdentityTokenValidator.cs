using VeloPass.Application.Identity;
using VeloPass.Domain.Abstractions;

namespace VeloPass.Application.Abstractions;

public interface IExternalIdentityTokenValidator
{
    Task<Result<ValidatedExternalIdentity>> ValidateAsync(
        ExternalIdentityProvider provider,
        string idToken,
        CancellationToken cancellationToken = default);
}