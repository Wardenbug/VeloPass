namespace VeloPass.Infrastructure.Authentication;

public sealed class JwtAuthOptions
{
    public string Issuer { get; init; } = String.Empty;
    public string Audience { get; init; } = String.Empty;
    public string Key { get; init; } = String.Empty;
    public int ExpirationInMinutes { get; init; }
    public int RefreshTokenExpirationInMinutes { get; init; }
}