using VeloPass.Application.Users.GetLoggedInUser;
using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Organizations;

namespace VeloPass.Application.Organizations.GetMembers;

public sealed class GetMembersByIdQueryHandler(IOrganizationMembersRepository membersRepository)
{
    public async Task<Result<IReadOnlyCollection<OrganizationMember>>>
        Handle(GetMembersByIdQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var member =
            await membersRepository.GetOrganizationMemberByUserIdAsync(query.UserId, query.OrganizationId,
                cancellationToken);

        if (member is null)
        {
            return Result.Invalid<IReadOnlyCollection<OrganizationMember>>("Organization not found");
        }
        
        var members = await 
            membersRepository.GetOrganizationMembersByOrganizationIdAsync(query.OrganizationId,
                cancellationToken);

        return Result.Ok(members);
    }
}