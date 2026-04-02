using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using VeloPass.Application.Abstractions;
using VeloPass.Application.Authentication;
using VeloPass.Infrastructure.Data;

namespace VeloPass.Infrastructure.Authentication;

public sealed class JwtService(
    IOptions<JwtAuthOptions> options,
    ApplicationIdentityDbContext applicationIdentityDbContext) : IJwtService
{
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;
    
    
    public AccessTokenDto GenerateJwtToken(TokenRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return new AccessTokenDto(CreateAccessToken(request), CreateRefreshToken());
    }

    public async Task<AccessTokenDto> RefreshJwtToken(string refreshToken, CancellationToken cancellationToken = default)
    {
        var token = await applicationIdentityDbContext.Set<RefreshTokenEntity>()
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);

        if (token is null)
        {
            throw new ArgumentNullException(nameof(refreshToken));
        }

        if (token.ExpiresAtUtc < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Refresh token is expired");
        }

        var accessToken = GenerateJwtToken(new TokenRequest(token.User.Id));

        token.Token = accessToken.RefreshToken;
        token.ExpiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtAuthOptions.RefreshTokenExpirationInMinutes);
        
        await applicationIdentityDbContext.SaveChangesAsync(cancellationToken);

        return accessToken;
    }

    private string CreateAccessToken(TokenRequest request)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtAuthOptions.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, request.UserId),
        ];

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtAuthOptions.ExpirationInMinutes),
            SigningCredentials = credentials,
            Audience = _jwtAuthOptions.Audience,
            Issuer = _jwtAuthOptions.Issuer
        };
        
        var handler = new JsonWebTokenHandler();
        
        string token = handler.CreateToken(tokenDescriptor);
        
        return token;
    }

    private static string CreateRefreshToken()
    {
        byte[] randomBytes = RandomNumberGenerator.GetBytes(32);
         
        return Convert.ToBase64String(randomBytes);
    }
}