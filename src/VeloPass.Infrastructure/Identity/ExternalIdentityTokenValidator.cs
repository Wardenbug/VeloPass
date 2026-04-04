using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using VeloPass.Application.Abstractions;
using VeloPass.Application.Identity;
using VeloPass.Domain.Abstractions;

namespace VeloPass.Infrastructure.Authentication;

public sealed class ExternalIdentityTokenValidator(IOptions<GoogleOptions> googleOptions) : IExternalIdentityTokenValidator
{
    public async Task<Result<ValidatedExternalIdentity>> ValidateAsync(ExternalIdentityProvider provider,
        string idToken,
        CancellationToken cancellationToken = default)
    {
        return provider switch
        {
            ExternalIdentityProvider.Google => await ValidateGoogleAsync(idToken),
            _ => throw new NotSupportedException($"Provider {provider} is not supported.")
        };
    }
    private async Task<Result<ValidatedExternalIdentity>> ValidateGoogleAsync(
        string idToken)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings()
        {
            Audience = new List<string> { googleOptions.Value.ClientId }
        };

        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
        
            var result = new ValidatedExternalIdentity(ExternalIdentityProvider.Google,
                payload.Subject,
                payload.Email,
                payload.Name,
                payload.EmailVerified);
        
            return Result.Ok(result);
        }
        catch (InvalidJwtException jwtException)
        {
            return Result.Invalid<ValidatedExternalIdentity>(jwtException.Message);
        }
    }
}