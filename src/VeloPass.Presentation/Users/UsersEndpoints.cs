using System.Security.Claims;
using VeloPass.Application.Users.GetLoggedInUser;
using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Users;
using VeloPass.Presentation.Extensions;
using Wolverine;

namespace VeloPass.Presentation.Users;

internal static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("users/me", GetMe)
            .RequireAuthorization();
        
        return routeBuilder;
    }


    private static async Task<IResult> GetMe(ClaimsPrincipal principal, IMessageBus messageBus, CancellationToken cancellationToken)
    {
        if (!principal.TryGetUserId(out var userId))
        {
            return Results.Unauthorized();
        }
        
        var result = await messageBus.InvokeAsync<Result<GetLoggedInUserDto>>(new GetLoggedInUserQuery(userId), cancellationToken);

        return result.ToHttpResult();
    }
}

