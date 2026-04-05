using VeloPass.Domain.Abstractions;

namespace VeloPass.Domain.Organizations;

public interface IOrganizationMembershipRepository
{
    Task<IReadOnlyList<OrganizationMembership>> GetOrganizationMembershipByUserIdAsync(string userId, CancellationToken cancellationToken = default);
}