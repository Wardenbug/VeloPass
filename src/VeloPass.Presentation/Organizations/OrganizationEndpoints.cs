using System.Security.Claims;
using VeloPass.Application.Organizations.ChangeMemberRole;
using VeloPass.Application.Organizations.Create;
using VeloPass.Application.Organizations.DeleteMember;
using VeloPass.Application.Organizations.GetById;
using VeloPass.Application.Organizations.GetMembers;
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

        routeBuilder.MapGet("organizations/{organizationId:guid}/members", GetMembersById)
            .RequireAuthorization();
        
        routeBuilder.MapGet("organizations/{organizationId:guid}", GetOrganizationById)
            .RequireAuthorization();
        
        routeBuilder.MapPatch("organizations/{organizationId:guid}/members/{id:guid}/roles", ChangeRole)
            .RequireAuthorization();
        
        return routeBuilder;
    }

    private static async Task<IResult> GetOrganizationById(Guid organizationId, IMessageBus messageBus,
        ClaimsPrincipal principal, CancellationToken cancellationToken)
    {
        if (!principal.TryGetUserId(out var userId))
        {
            return Results.Unauthorized();
        }

        var result =
            await messageBus.InvokeAsync<Result<Organization>>(new GetOrganizationByIdQuery(organizationId, userId),
                cancellationToken);


        return result.ToHttpResult();
    }

    private static async Task<IResult> GetMembersById(Guid organizationId, IMessageBus messageBus,
        ClaimsPrincipal principal, CancellationToken cancellationToken)
    {
        if (!principal.TryGetUserId(out var userId))
        {
            return Results.Unauthorized();
        }
        
        var result =
            await messageBus.InvokeAsync<Result<IReadOnlyCollection<OrganizationMembership>>>(
                new GetMembersByIdQuery(organizationId, userId), cancellationToken);

        return result.ToHttpResult();
    }

    private static async Task<IResult> Create(
        CreateOrganizationRequest request, 
        ClaimsPrincipal principal, 
        IMessageBus messageBus, 
        CancellationToken cancellationToken)
    {
        if (!principal.TryGetUserId(out var userId))
        {
            return Results.Unauthorized();
        }
        
        var result = await messageBus.InvokeAsync<Result<Organization>>(
            new CreateOrganizationCommand(request.Name, userId), cancellationToken);

        return result.ToHttpResult();
    }

    private static async Task<IResult>
        DeleteMember(Guid organizationId, Guid id, IMessageBus messageBus, ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {

        if (!principal.TryGetUserId(out var userId))
        {
            return Results.Unauthorized();
        }
        
        var result = await messageBus.InvokeAsync<Result<bool>>(new DeleteMemberCommand(userId, id, organizationId), cancellationToken);

        return result.ToHttpResult();
    }

    private static async Task<IResult> ChangeRole(ChangeRoleRequest request, Guid organizationId, Guid id,
        ClaimsPrincipal principal,
        IMessageBus messageBus, CancellationToken cancellationToken)
    {
        if (!principal.TryGetUserId(out var userId))
        {
            return Results.Unauthorized();
        }

        var result = await messageBus.InvokeAsync<Result<bool>>(new ChangeMemberRoleCommand(userId, id, organizationId, request.Role),
            cancellationToken);

        return result.ToHttpResult();
    }
}