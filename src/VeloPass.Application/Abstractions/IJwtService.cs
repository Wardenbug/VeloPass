using VeloPass.Application.Authentication;

namespace VeloPass.Application.Abstractions;

public interface IJwtService
{
    AccessTokenDto GenerateJwtToken(TokenRequest request);
    Task<AccessTokenDto> RefreshJwtToken(string refreshToken, CancellationToken cancellationToken = default);
}