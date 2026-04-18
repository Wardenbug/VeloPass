using Microsoft.EntityFrameworkCore;
using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Organizations;
using VeloPass.Domain.Users;
using VeloPass.Infrastructure.Data;

namespace VeloPass.Infrastructure.Organizations;

internal sealed class OrganizationRepository(ApplicationDbContext dbContext) : IOrganizationRepository
{
    public void Add(Organization organization)
    {
        dbContext.Add(organization);
    }

    public async Task<Result<Organization>> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var org = await dbContext.Set<Organization>()
            .FirstOrDefaultAsync(
            o => o.Name == name, cancellationToken);

        if (org is null)
        {
            return Result.NotFound<Organization>("Organization not found");
        }
        
        return Result.Ok(org);
    }

    public async Task<Result<OrganizationMembership>> FindMembershipByUserIdAsync(
        Guid userId, 
        Guid organizationId, CancellationToken cancellationToken = default)
    {
        var member = await dbContext.Set<OrganizationMembership>()
            .FirstOrDefaultAsync(m =>
                    m.UserId == userId && m.OrganizationId == organizationId,
                cancellationToken);

        if (member is null)
        {
            return Result.NotFound<OrganizationMembership>("Member not found");
        }

        return Result.Ok(member);
    }

    public async Task<Result<OrganizationMembership>> FindMembershipByEmailAsync(string email, Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        var member = await dbContext.Set<OrganizationMembership>()
            .Where(member => member.OrganizationId == organizationId)
            .Join(
                dbContext.Set<User>(),
                member => member.UserId,
                user => user.Id,
                (member, user) => new {user, member})
            .Where(mu => mu.user.Email == email)
            .Select(mu => mu.member)
            .FirstOrDefaultAsync(cancellationToken);

        if (member is null)
        {
            return Result.NotFound<OrganizationMembership>("Member not found");
        }
        
        return Result.Ok(member);
    }
}