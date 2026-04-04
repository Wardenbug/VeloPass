using VeloPass.Application.Authentication;
using VeloPass.Domain.Abstractions;

namespace VeloPass.Application.Abstractions;

public interface IJwtService
{
    AccessTokenDto GenerateJwtToken(TokenRequest request);
    Task<Result<AccessTokenDto>> RefreshJwtToken(string refreshToken, CancellationToken cancellationToken = default);
    
    Task<Result<bool>> RevokeJwtToken(string refreshToken, CancellationToken cancellationToken = default);
}