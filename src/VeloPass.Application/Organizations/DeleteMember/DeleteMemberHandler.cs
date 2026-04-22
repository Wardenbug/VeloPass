using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Organizations;

namespace VeloPass.Application.Organizations.DeleteMember;

public class DeleteMemberHandler(IUnitOfWork unitOfWork,
    IOrganizationRepository organizationRepository)
{
    public async Task<Result<bool>> Handle(DeleteMemberCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var orgResult = await organizationRepository.FindByIdAsync(command.OrganizationId, cancellationToken);
        
        if (!orgResult.IsSuccess)
        {
            return Result.NotFound<bool>("Organization not found");
        }
        
        var result = orgResult.Value.RemoveMember(command.CallerId, command.MemberId);
        
        if (!result.IsSuccess)
        {
            return result;
        }
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Ok(true);
    }
}