using Microsoft.EntityFrameworkCore;
using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Organizations;
using VeloPass.Infrastructure.Data;

namespace VeloPass.Infrastructure.Organizations;

public sealed class OrganizationMembershipRepository(ApplicationDbContext dbContext) : IOrganizationMembershipRepository
{
    public async Task<IReadOnlyList<OrganizationMembership>> GetOrganizationMembershipByUserIdAsync(
        string userId, 
        CancellationToken cancellationToken = default)
    {
        var result = await dbContext.Set<OrganizationMembership>()
            .AsNoTracking()
            .Where(om => om.UserId.ToString() == userId)
            .ToListAsync(cancellationToken);


        return result;
    }
}