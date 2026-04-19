using System.Security.Claims;
using VeloPass.Application.Organizations.Create;
using VeloPass.Application.Organizations.DeleteMember;
using VeloPass.Application.Organizations.GetById;
using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Organizations;
using VeloPass.Presentation.Extensions;
using Wolverine;

namespace VeloPass.Presentation.Organizations;

internal static class OrganizationEndpoints
{
    public static IEndpointRouteBuilder MapOrganizationEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost("organizations", Create)
            .RequireAuthorization();
        routeBuilder.MapDelete("organizations/{organizationId:guid}/members/{id:guid}", DeleteMember)
            .RequireAuthorization();

        routeBuilder.MapGet("organizations/{organizationId:guid}", GetOrganizationById);
        return routeBuilder;
    }

    private static async Task<IResult> GetOrganizationById(Guid organizationId, IMessageBus messageBus,
        ClaimsPrincipal principal, CancellationToken cancellationToken)
    {
        if (!principal.TryGetUserId(out var userId))
        {
            return Results.Unauthorized();
        }

        if (!Guid.TryParse(userId, out var idGuid))
        {
            return Results.BadRequest();
        }

        var organization =
            await messageBus.InvokeAsync<Result<Organization>>(new GetOrganizationByIdQuery(organizationId, idGuid),
                cancellationToken);

        
        if (!organization.IsSuccess)
        {
            return Results.BadRequest(organization.Error.Message);
        }
        
        return Results.Ok(organization.Value);
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

    private static async Task<IResult>
        DeleteMember(Guid organizationId, Guid id, IMessageBus messageBus, ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {

        if (!principal.TryGetUserId(out var userId))
        {
            return Results.Unauthorized();
        }

        if (!Guid.TryParse(userId, out var idGuid))
        {
            return Results.BadRequest();
        }

        var result = await messageBus.InvokeAsync<Result<bool>>(new DeleteMemberCommand(idGuid, id, organizationId), cancellationToken);

        if (!result.IsSuccess)
        {
            return Results.BadRequest(result.Error.Message);
        }

        return Results.NoContent();
    }
}