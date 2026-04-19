using VeloPass.Domain.Abstractions;

namespace VeloPass.Domain.Organizations;

public interface IOrganizationRepository
{
    void Add(Organization organization);
    
    Task<Result<Organization>> FindByNameAsync(string name, CancellationToken cancellationToken = default);
    
    Task<Result<Organization>> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<Result<OrganizationMembership>> FindMembershipByUserIdAsync(Guid userId, Guid organizationId, CancellationToken cancellationToken = default);
    
    Task<Result<OrganizationMembership>> FindMembershipByEmailAsync(string email, Guid organizationId, CancellationToken cancellationToken = default);
}