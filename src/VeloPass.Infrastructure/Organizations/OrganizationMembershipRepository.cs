using Microsoft.EntityFrameworkCore;
using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Organizations;
using VeloPass.Domain.Users;
using VeloPass.Infrastructure.Data;

namespace VeloPass.Infrastructure.Organizations;

public sealed class OrganizationMembershipRepository(ApplicationDbContext dbContext) : IOrganizationMembershipRepository
{
    public async Task<IReadOnlyList<OrganizationMembership>> GetOrganizationsMembershipByUserIdAsync(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        var result = await dbContext.Set<OrganizationMembership>()
            .AsNoTracking()
            .Where(om => om.UserId == userId)
            .ToListAsync(cancellationToken);
        
        return result;
    }

    public async Task<IReadOnlyCollection<OrganizationMembership>> GetOrganizationsMembershipByOrganizationIdAsync(
        Guid organizationId, CancellationToken cancellationToken = default)
    {
        var result = await dbContext.Set<OrganizationMembership>()
            .AsNoTracking()
            .Where(om => om.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);

        return result;
    }

    public async Task<OrganizationMembership?> GetOrganizationMembershipByUserIdAsync(Guid userId, Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        var result = await dbContext.Set<OrganizationMembership>()
            .Where(om => om.UserId == userId && om.OrganizationId == organizationId)
            .FirstOrDefaultAsync(cancellationToken);
        
        return result;
    }

    public async Task<bool> DeleteOrganizationMembershipByUserIdAsync(Guid userId, Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Set<OrganizationMembership>()
            .Where(om => om.UserId == userId && om.OrganizationId == organizationId)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return false;
        }

        if (user.Role == OrganizationRole.Owner)
        {
            return false;
        }
        
        dbContext.Remove(user);

        return true;
    }
}