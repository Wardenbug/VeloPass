namespace VeloPass.Application.Organizations.GetMembers;

public sealed record GetMembersByIdQuery(Guid OrganizationId, Guid UserId);