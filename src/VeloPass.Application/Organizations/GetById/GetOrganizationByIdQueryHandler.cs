using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Organizations;

namespace VeloPass.Application.Organizations.GetById;

public class GetOrganizationByIdQueryHandler(
    IOrganizationRepository repository,
    IOrganizationMembershipRepository membershipRepository)
{
    public async Task<Result<Organization>> Handle(GetOrganizationByIdQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var member =
            await membershipRepository.GetOrganizationMembershipByUserIdAsync(query.UserId, query.OrganizationId,
                cancellationToken);

        if (member is null)
        {
            return Result.Invalid<Organization>("Organization not found");
        }
        
        var organization = await repository.FindByIdAsync(query.OrganizationId, cancellationToken);

        if (!organization.IsSuccess)
        {
            return Result.NotFound<Organization>("Organization not found");
        }
        
        return Result.Ok(organization.Value);
    }
}