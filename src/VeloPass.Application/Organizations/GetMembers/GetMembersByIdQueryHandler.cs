using VeloPass.Application.Users.GetLoggedInUser;
using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Organizations;

namespace VeloPass.Application.Organizations.GetMembers;

public sealed class GetMembersByIdQueryHandler(IOrganizationMembershipRepository membershipRepository)
{
    public async Task<Result<IReadOnlyCollection<OrganizationMembership>>>
        Handle(GetMembersByIdQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var member =
            await membershipRepository.GetOrganizationMembershipByUserIdAsync(query.UserId, query.OrganizationId,
                cancellationToken);

        if (member is null)
        {
            return Result.Invalid<IReadOnlyCollection<OrganizationMembership>>("Organization not found");
        }
        
        var members = await 
            membershipRepository.GetOrganizationsMembershipByOrganizationIdAsync(query.OrganizationId,
                cancellationToken);

        return Result.Ok(members);
    }
}