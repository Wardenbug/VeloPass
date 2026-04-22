using Microsoft.EntityFrameworkCore;
using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Organizations;
using VeloPass.Infrastructure.Data;

namespace VeloPass.Infrastructure.Organizations;

internal sealed class OrganizationRepository(ApplicationDbContext applicationDbContext) : IOrganizationRepository
{
    private readonly DbSet<Organization> _organizationDbSet = applicationDbContext.Set<Organization>();
    
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
            .Include(o => o.Members)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        
        if (org is null)
        {
            return Result.NotFound<Organization>("Organization not found");
        }
        
        return Result.Ok(org);
    }
}