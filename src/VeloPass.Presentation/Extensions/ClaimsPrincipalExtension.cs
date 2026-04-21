using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace VeloPass.Presentation.Extensions;

internal static class ClaimsPrincipalExtension
{
    public static bool TryGetUserId(this ClaimsPrincipal principal, out Guid userId)
    {
        ArgumentNullException.ThrowIfNull(principal);
        var sub = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(sub, out userId);
    }
}