using VeloPass.Domain.Abstractions;

namespace VeloPass.Domain.Organizations;

public interface IOrganizationRepository
{
    void Add(Organization organization);
    
    Task<Result<Organization>> FindByNameAsync(string name, CancellationToken cancellationToken = default);
    
    Task<Result<Organization>> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
}