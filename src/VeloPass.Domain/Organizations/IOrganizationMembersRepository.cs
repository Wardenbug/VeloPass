using VeloPass.Domain.Abstractions;

namespace VeloPass.Domain.Organizations;

public interface IOrganizationMembersRepository
{
    Task<IReadOnlyList<OrganizationMember>> GetOrganizationMembersByUserIdAsync(Guid userId,
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<OrganizationMember>> GetOrganizationMembersByOrganizationIdAsync(
        Guid organizationId, CancellationToken cancellationToken = default);
    
    Task<OrganizationMember?> GetOrganizationMemberByUserIdAsync(Guid userId, Guid organizationId,
        CancellationToken cancellationToken = default);
    
    Task<OrganizationMember?> FindMemberByEmailAsync(string email, Guid organizationId,
        CancellationToken cancellationToken = default);
}