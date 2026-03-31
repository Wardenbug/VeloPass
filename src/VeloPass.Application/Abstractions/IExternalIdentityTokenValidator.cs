using VeloPass.Application.Identity;

namespace VeloPass.Application.Abstractions;

public interface IExternalIdentityTokenValidator
{
    Task<ValidatedExternalIdentity> ValidateAsync(
        ExternalIdentityProvider provider,
        string idToken,
        CancellationToken cancellationToken = default);
}