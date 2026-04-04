using VeloPass.Application.Authentication;
using VeloPass.Application.Users.LoginUser;
using VeloPass.Application.Users.LogoutUser;
using VeloPass.Application.Users.RefreshUser;
using VeloPass.Application.Users.RegisterUser;
using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Users;
using Wolverine;

namespace VeloPass.Presentation.Authentication;

internal static class AuthenticationEndpoints
{
    public static IEndpointRouteBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost("auth/register", Register);
        routeBuilder.MapPost("auth/refresh", RefreshToken);
        routeBuilder.MapPost("auth/login", Login);
        routeBuilder.MapPost("auth/logout", Logout);
        
        return routeBuilder;
    }

    private static async Task<IResult> Login(
        LoginUserCommand command, 
        IMessageBus messageBus,
        CancellationToken cancellationToken)
    {
        var result = await messageBus.InvokeAsync<Result<AccessTokenDto>>(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return Results.Problem(title: result.Error.Message);
        }
        
        return Results.Ok(result.Value);
    }
    
    private static async Task<IResult> Logout(
        LogoutUserCommand command, 
        IMessageBus messageBus,
        CancellationToken cancellationToken)
    {
        var result = await messageBus.InvokeAsync<Result<bool>>(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return Results.Problem(title: result.Error.Message);
        }
        
        return Results.Ok(result.Value);
    }

    private static async Task<IResult> Register(
        RegisterUserRequest request, 
        IMessageBus messageBus,
        CancellationToken cancellationToken)
    {
        var result = await messageBus.InvokeAsync<Result<AccessTokenDto>>(
            new RegisterUserCommand(request.IdToken), cancellationToken);

        if (!result.IsSuccess)
        {
            return Results.Problem(title: result.Error.Message);
        }

        return Results.Ok(result.Value);
    }

    private static async Task<IResult> RefreshToken(
        RefreshTokenRequest request, 
        IMessageBus messageBus,
        CancellationToken cancellationToken)
    {
        var result = await messageBus.InvokeAsync<Result<AccessTokenDto>>(
            new RefreshUserCommand(request.RefreshToken), cancellationToken);
        
        if (!result.IsSuccess)
        {
            return Results.Problem(title: result.Error.Message);
        }

        return Results.Ok(result.Value);
    }
}