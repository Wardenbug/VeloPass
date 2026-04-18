using VeloPass.Domain.Organizations;

namespace VeloPass.Application.Invites.CreateInvite;

public sealed record CreateInviteCommand(string Email, OrganizationRole Role, Guid InvitedByUserId, Guid OrganizationId);