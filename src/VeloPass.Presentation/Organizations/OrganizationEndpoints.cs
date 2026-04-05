using System.Security.Claims;
using VeloPass.Application.Organizations.Create;
using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Organizations;
using Wolverine;

namespace VeloPass.Presentation.Organizations;

internal static class OrganizationEndpoints
{
    public static IEndpointRouteBuilder MapOrganizationEndpoints(this IEndpointRouteBuilder routeBuilder)
    {

        routeBuilder.MapPost("organizations", Create)
            .RequireAuthorization();
        return routeBuilder;
    }

    private static async Task<IResult> Create(
        CreateOrganizationRequest request, 
        ClaimsPrincipal principal, 
        IMessageBus messageBus, 
        CancellationToken cancellationToken)
    {
        var sub = principal.FindFirst(ClaimTypes.NameIdentifier);

        if (sub is null)
        {
            return Results.BadRequest();
        }
        
        var result = await messageBus.InvokeAsync<Result<Organization>>(
            new CreateOrganizationCommand(request.Name, sub.Value), cancellationToken);

        if (!result.IsSuccess)
        {
            return Results.BadRequest();
        }
        
        return Results.Ok(result.Value);
    }
}