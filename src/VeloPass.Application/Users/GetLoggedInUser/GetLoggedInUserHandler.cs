using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Organizations;
using VeloPass.Domain.Users;

namespace VeloPass.Application.Users.GetLoggedInUser;

public class GetLoggedInUserHandler(
    IUserRepository userRepository,
    IOrganizationMembersRepository organizationMembersRepository)
{
    public async Task<Result<GetLoggedInUserDto>> Handle(GetLoggedInUserQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var userResult = await userRepository.FindByIdAsync(query.UserId.ToString(), cancellationToken);

        if (!userResult.IsSuccess)
        {
            return Result.NotFound<GetLoggedInUserDto>("User not found");
        }
        
        var organizationMembers = await organizationMembersRepository
            .GetOrganizationMembersByUserIdAsync(query.UserId, cancellationToken);

        var items = organizationMembers
            .Select(om => new OrganizationMemberItemDto(om.OrganizationId.ToString(), om.Role.ToString()))
            .ToList();
        
        return Result.Ok(new GetLoggedInUserDto(
            userResult.Value.Id.ToString(), 
            userResult.Value.Email, 
            userResult.Value.Name,
            items));
    }
}