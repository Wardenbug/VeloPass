using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Organizations;

namespace VeloPass.Application.Organizations.ChangeMemberRole;

public sealed class ChangeMemberRoleCommandHandler(IOrganizationRepository organizationRepository, IUnitOfWork unitOfWork)
{
    public async Task<Result<bool>> Handle(ChangeMemberRoleCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var orgResult = await organizationRepository.FindByIdAsync(command.OrganizationId, cancellationToken);
        
        if (!orgResult.IsSuccess)
        {
            return Result.NotFound<bool>("Organization not found");
        }
        
        var result = orgResult.Value.ChangeMemberRole(command.UserId, command.MemberId, command.Role);
        
        if (!result.IsSuccess)
        {
            return result;
        }
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Ok(true);
    }
}