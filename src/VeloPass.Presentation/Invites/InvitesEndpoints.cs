using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using VeloPass.Application.Authentication;
using VeloPass.Application.Invites.ApplyInvite;
using VeloPass.Application.Invites.CancelInvite;
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

        endpoints.MapDelete("invites/{id:guid}", CancelInvite);

        endpoints.MapPost("invites/accept", AcceptInvite)
            .AllowAnonymous();
        
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
             new CreateInviteCommand(request.Email, OrganizationRole.Member, userId,
                 request.OrganizationId), cancellationToken);

         if (!invite.IsSuccess)
         {
             return Results.BadRequest(invite.Error.Message);
         }
         
         return Results.Ok(invite.Value);
    }

    private static async Task<IResult> AcceptInvite([FromQuery] string token, IMessageBus messageBus,
        CancellationToken cancellationToken)
    {
        var result = await messageBus.InvokeAsync<Result<AccessTokenDto>>(new AcceptInviteCommand(token), cancellationToken);

        if (!result.IsSuccess)
        {
            return Results.BadRequest(result.Error.Message);
        }
        
        return Results.Ok(result.Value);
    }

    private static async Task<IResult> CancelInvite(Guid id, IMessageBus messageBus, ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        if (!principal.TryGetUserId(out var userId))
        {
            return Results.Unauthorized();
        }

        var result = await messageBus.InvokeAsync<Result<bool>>(new CancelInviteCommand(id, userId), cancellationToken);

        if (!result.IsSuccess)
        {
            return Results.BadRequest(result.Error.Message);
        }

        return Results.NoContent();
    }
}