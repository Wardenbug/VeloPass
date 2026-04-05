using System.Security.Claims;
using VeloPass.Application.Users.GetLoggedInUser;
using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Users;
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
        var sub = principal.FindFirst(ClaimTypes.NameIdentifier);

        if (sub is null)
        {
            return Results.BadRequest();
        }

        var userResult = await messageBus.InvokeAsync<Result<GetLoggedInUserDto>>(new GetLoggedInUserQuery(sub.Value), cancellationToken);

        if (!userResult.IsSuccess)
        {
            return Results.BadRequest(userResult.Error);
        }
        
        return Results.Ok(userResult.Value);
    }
}

