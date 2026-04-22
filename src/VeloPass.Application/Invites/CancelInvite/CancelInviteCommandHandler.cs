using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Invites;
using VeloPass.Domain.Organizations;

namespace VeloPass.Application.Invites.CancelInvite;

public sealed class CancelInviteCommandHandler(
    IInviteRepository inviteRepository,
    IUnitOfWork unitOfWork,
    IOrganizationMembersRepository memberRepository)
{
    public async Task<Result<bool>> Handle(CancelInviteCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var inviteResult = await inviteRepository.GetByIdAsync(command.InviteId, cancellationToken);
        
        if (!inviteResult.IsSuccess)
        {
            return Result.NotFound<bool>("Invite not found");
        }
        
        var invite = inviteResult.Value;
        
        var member =
            await memberRepository.GetOrganizationMemberByUserIdAsync(command.UserId, invite.OrganizationId,
                cancellationToken);
        
        if (member is null)
        {
            return Result.Invalid<bool>("User is not a member of organization");
        }
        
        var isInviter = invite.InvitedByUserId == command.UserId;
        
        if (!member.CanManageMembers() && !isInviter)
        {
            return Result.Invalid<bool>("User is not allowed to cancel this invite");
        }
        
        inviteRepository.Remove(invite);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Ok(true);
        
    }
}