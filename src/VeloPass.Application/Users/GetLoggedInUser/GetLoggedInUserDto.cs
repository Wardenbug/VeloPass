using VeloPass.Domain.Organizations;

namespace VeloPass.Application.Users.GetLoggedInUser;

public record GetLoggedInUserDto(string UserId, string Email, string Name, IReadOnlyList<OrganizationMembershipItemDto> Organizations);