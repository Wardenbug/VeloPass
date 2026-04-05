using Microsoft.EntityFrameworkCore;
using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Organizations;
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
}