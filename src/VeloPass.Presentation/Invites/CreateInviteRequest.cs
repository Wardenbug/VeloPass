namespace VeloPass.Presentation.Invites;

internal sealed record CreateInviteRequest(string Email, Guid OrganizationId);