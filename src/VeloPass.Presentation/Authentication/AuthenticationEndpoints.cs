using VeloPass.Application.Authentication;
using VeloPass.Application.Users.RegisterUser;
using VeloPass.Domain.Users;
using Wolverine;

namespace VeloPass.Presentation.Authentication;

internal static class AuthenticationEndpoints
{
    public static IEndpointRouteBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost("auth/register", Register);
        routeBuilder.MapPost("auth/refresh", RefreshToken);
        
        return routeBuilder;
    }

    private static async Task<IResult> Register(
        RegisterUserRequest request, 
        IMessageBus messageBus,
        CancellationToken cancellationToken)
    {
        var result = await messageBus.InvokeAsync<AccessTokenDto>(
            new RegisterUserCommand(request.IdToken), cancellationToken);

        return Results.Ok(result);
    }

    private static async Task<AccessTokenDto> RefreshToken(
        RefreshTokenRequest request, 
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}