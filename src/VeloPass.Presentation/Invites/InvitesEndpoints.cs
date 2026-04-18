using System.Security.Claims;
using VeloPass.Application.Invites.CreateInvite;
using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Invites;
using VeloPass.Domain.Organizations;
using VeloPass.Presentation.Extensions;
using Wolverine;

namespace VeloPass.Presentation.Invites;

internal static class InvitesEndpoints
{
    public static IEndpointRouteBuilder MapInvitesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("invites", Create)
            .RequireAuthorization();
        
        return endpoints;
    }

    private static async Task<IResult> Create(
        CreateInviteRequest request,
        ClaimsPrincipal principal,
        IMessageBus messageBus,
        CancellationToken cancellationToken)
    {
         if(!principal.TryGetUserId(out var userId))
         {
             return Results.Unauthorized();
         }
         
         var invite = await messageBus.InvokeAsync<Result<Invite>>(
             new CreateInviteCommand(request.Email, OrganizationRole.Member, Guid.Parse(userId),
                 request.OrganizationId), cancellationToken);

         if (!invite.IsSuccess)
         {
             return Results.BadRequest(invite.Error.Message);
         }
         
         return Results.Ok(invite.Value);
    }
}