using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace VeloPass.Presentation.Extensions;

internal static class ClaimsPrincipalExtension
{
    public static bool TryGetUserId(this ClaimsPrincipal principal, [NotNullWhen(true)] out string? userId)
    {
        ArgumentNullException.ThrowIfNull(principal);
        var sub = principal.FindFirst(ClaimTypes.NameIdentifier);
        userId = sub?.Value;
        return !string.IsNullOrEmpty(userId);
    }
}