using VeloPass.Domain.Organizations;

namespace VeloPass.Application.Organizations.ChangeMemberRole;

public sealed record ChangeMemberRoleCommand(Guid UserId, Guid MemberId, Guid OrganizationId, OrganizationRole Role);