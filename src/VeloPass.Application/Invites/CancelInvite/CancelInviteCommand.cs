namespace VeloPass.Application.Invites.CancelInvite;

public sealed record CancelInviteCommand(Guid InviteId, Guid UserId);