using Microsoft.EntityFrameworkCore;
using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Organizations;
using VeloPass.Domain.Users;
using VeloPass.Infrastructure.Data;

namespace VeloPass.Infrastructure.Organizations;

public sealed class OrganizationMembersRepository(ApplicationDbContext dbContext) : IOrganizationMembersRepository
{
    public async Task<IReadOnlyList<OrganizationMember>> GetOrganizationMembersByUserIdAsync(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        var result = await dbContext.Set<OrganizationMember>()
            .AsNoTracking()
            .Where(om => om.UserId == userId)
            .ToListAsync(cancellationToken);
        
        return result;
    }

    public async Task<IReadOnlyCollection<OrganizationMember>> GetOrganizationMembersByOrganizationIdAsync(
        Guid organizationId, CancellationToken cancellationToken = default)
    {
        var result = await dbContext.Set<OrganizationMember>()
            .AsNoTracking()
            .Where(om => om.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);

        return result;
    }

    public async Task<OrganizationMember?> GetOrganizationMemberByUserIdAsync(Guid userId, Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        var result = await dbContext.Set<OrganizationMember>()
            .Where(om => om.UserId == userId && om.OrganizationId == organizationId)
            .FirstOrDefaultAsync(cancellationToken);
        
        return result;
    }

    public async Task<OrganizationMember?> FindMemberByEmailAsync(string email, Guid organizationId, CancellationToken cancellationToken = default)
    {
        var result = await dbContext.Set<OrganizationMember>()
            .Join(dbContext.Set<User>(),
                om => om.UserId,
                user => user.Id,
                (member, user) => new { user, member })
            .Where(m => m.user.Email == email &&  m.member.OrganizationId == organizationId)
            .Select((x) => x.member)
            .FirstOrDefaultAsync(cancellationToken);

        return result;
    }
}