using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Organizations;

namespace VeloPass.Application.Organizations.ChangeMemberRole;

public sealed class ChangeMemberRoleCommandHandler(IOrganizationMembershipRepository membershipRepository, IUnitOfWork unitOfWork)
{
    public async Task<Result<bool>> Handle(ChangeMemberRoleCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var currentMember =
            await membershipRepository.GetOrganizationMembershipByUserIdAsync(command.UserId,
                command.OrganizationId, cancellationToken);

        if (currentMember is null)
        {
            return Result.Invalid<bool>("You don’t have access to this organization");
        }

        if (currentMember.Role != OrganizationRole.Admin && currentMember.Role != OrganizationRole.Owner)
        {
            return Result.Invalid<bool>("You don’t have permission to manage members");
        }

        if (command.Role == OrganizationRole.Owner)
        {
            return Result.Invalid<bool>("You cannot change the member's role");
        }

        var member = await 
            membershipRepository.GetOrganizationMembershipByUserIdAsync(command.MemberId, command.OrganizationId,
                cancellationToken);
        
        if (member is null)
        {
            return Result.Invalid<bool>("Member not found");
        }
        
        if (member.Role == OrganizationRole.Owner)
        {
            return Result.Invalid<bool>("You cannot change the member's role");
        }

        member.ChangeRole(command.Role);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Ok(true);
    }
}