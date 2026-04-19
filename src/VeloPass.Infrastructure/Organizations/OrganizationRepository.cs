using Microsoft.EntityFrameworkCore;
using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Organizations;
using VeloPass.Domain.Users;
using VeloPass.Infrastructure.Data;

namespace VeloPass.Infrastructure.Organizations;

internal sealed class OrganizationRepository(ApplicationDbContext applicationDbContext) : IOrganizationRepository
{

    private readonly DbSet<Organization> _organizationDbSet = applicationDbContext.Set<Organization>();
    private readonly DbSet<OrganizationMembership> _organizationMembershipDbSet = applicationDbContext.Set<OrganizationMembership>();
    
    public void Add(Organization organization)
    {
        _organizationDbSet.Add(organization);
    }

    public async Task<Result<Organization>> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var org = await _organizationDbSet
            .FirstOrDefaultAsync(
            o => o.Name == name, cancellationToken);

        if (org is null)
        {
            return Result.NotFound<Organization>("Organization not found");
        }
        
        return Result.Ok(org);
    }

    public async Task<Result<Organization>> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var org  = await _organizationDbSet
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
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
        var member = await _organizationMembershipDbSet
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
        var member = await _organizationMembershipDbSet
            .Where(member => member.OrganizationId == organizationId)
            .Join(
                applicationDbContext.Set<User>(),
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