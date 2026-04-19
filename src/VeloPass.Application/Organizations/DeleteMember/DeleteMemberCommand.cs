namespace VeloPass.Application.Organizations.DeleteMember;

public sealed record DeleteMemberCommand(Guid CallerId, Guid MemberId, Guid OrganizationId);