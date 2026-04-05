using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Organizations;
using VeloPass.Domain.Users;

namespace VeloPass.Application.Users.GetLoggedInUser;

public class GetLoggedInUserHandler(
    IUserRepository userRepository,
    IOrganizationMembershipRepository organizationMembershipRepository)
{
    public async Task<Result<GetLoggedInUserDto>> Handle(GetLoggedInUserQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var userResult = await userRepository.FindByIdAsync(query.UserId, cancellationToken);

        if (!userResult.IsSuccess)
        {
            return Result.NotFound<GetLoggedInUserDto>("User not found");
        }
        
        var organizationMembership = await organizationMembershipRepository
            .GetOrganizationMembershipByUserIdAsync(query.UserId, cancellationToken);

        var items = organizationMembership
            .Select(om => new OrganizationMembershipItemDto(om.OrganizationId.ToString(), om.Role.ToString()))
            .ToList();
        
        return Result.Ok(new GetLoggedInUserDto(
            userResult.Value.Id.ToString(), 
            userResult.Value.Email, 
            userResult.Value.Name,
            items));
    }
}