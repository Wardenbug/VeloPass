using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Organizations;

namespace VeloPass.Application.Organizations.DeleteMember;

public class DeleteMemberHandler(IUnitOfWork unitOfWork,
    IOrganizationMembershipRepository membershipRepository)
{
    public async Task<Result<bool>> Handle(DeleteMemberCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var currentMember =
            await membershipRepository.GetOrganizationMembershipByUserIdAsync(command.CallerId,
                command.OrganizationId, cancellationToken);

        if (currentMember is null)
        {
            return Result.Invalid<bool>("You don’t have access to this organization");
        }

        if (currentMember.Role != OrganizationRole.Admin && currentMember.Role != OrganizationRole.Owner)
        {
            return Result.Invalid<bool>("You don’t have permission to manage members");
        }

        var result =
            await membershipRepository.DeleteOrganizationMembershipByUserIdAsync(command.MemberId,
                command.OrganizationId, cancellationToken);

        if (!result)
        {
            return Result.Invalid<bool>("Member could not be removed");
        }
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Ok(true);
    }
}