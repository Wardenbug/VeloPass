using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Organizations;

namespace VeloPass.Application.Organizations.Create;

public sealed class CreateOrganizationHandler(
    IUnitOfWork unitOfWork,
    IOrganizationRepository repository
    )
{
    public async Task<Result<Organization>> Handle(
        CreateOrganizationCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var existingOrganizationResult = await repository.FindByNameAsync(command.Name, cancellationToken);

        if (existingOrganizationResult.IsSuccess)
        {
            return Result.Invalid<Organization>("Organization with this name already exists");
        }

        var organization = Organization.Create(command.Name, Guid.Parse(command.OwnerId));
        
        repository.Add(organization);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Ok(organization);
    }
}