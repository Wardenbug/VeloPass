using VeloPass.Domain.Abstractions;

namespace VeloPass.Domain.Organizations;

public interface IOrganizationMembershipRepository
{
    Task<IReadOnlyList<OrganizationMembership>> GetOrganizationsMembershipByUserIdAsync(Guid userId,
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<OrganizationMembership>> GetOrganizationsMembershipByOrganizationIdAsync(
        Guid organizationId, CancellationToken cancellationToken = default);
    
    Task<OrganizationMembership?> GetOrganizationMembershipByUserIdAsync(Guid userId, Guid organizationId,
        CancellationToken cancellationToken = default);
    
    Task<bool> DeleteOrganizationMembershipByUserIdAsync(Guid userId, Guid organizationId,
        CancellationToken cancellationToken = default);
}