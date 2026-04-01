using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using VeloPass.Application.Abstractions;
using VeloPass.Application.Authentication;

namespace VeloPass.Infrastructure.Authentication;

public class JwtService(IOptions<JwtAuthOptions> options) : IJwtService
{
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;
    
    
    public AccessTokenDto GenerateJwtToken(TokenRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return new AccessTokenDto(CreateAccessToken(request), CreateRefreshToken());
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