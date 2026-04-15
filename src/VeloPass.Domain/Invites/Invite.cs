using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Invites.Events;
using VeloPass.Domain.Organizations;

namespace VeloPass.Domain.Invites;

public sealed class Invite : Entity
{
    private Invite(Guid id, string email, OrganizationRole role, string invitedByUserId, Guid organizationId,
        string tokenHash, DateTime expiredAtUtc) : base(id)
    {
        Email = email;
        OrganizationRole = role;
        InvitedByUserId = invitedByUserId;
        OrganizationId = organizationId;
        TokenHash = tokenHash;
        ExpiredAtUtc = expiredAtUtc;
    }

    private Invite()
    {
    }

    public string Email { get; private set; } = string.Empty;
    public OrganizationRole OrganizationRole { get; private set; }
    public Guid OrganizationId { get; private set; }
    public string InvitedByUserId { get; private set; } = string.Empty;
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiredAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? AcceptedAtUtc { get; private set; }

    public static Invite Create(string email, OrganizationRole role, string invitedByUserId, Guid organizationId,
        string tokenHash, DateTime expiredAtUtc)
    {
        var invite =
            new Invite(Guid.CreateVersion7(), email, role, invitedByUserId, organizationId, tokenHash, expiredAtUtc)
            {
                CreatedAtUtc = DateTime.UtcNow
            };

        invite.RaiseDomainEvent(new InviteCreatedDomainEvent(invite.Email, invite.OrganizationId.ToString()));

        return invite;
    }

    public void Accept()
    {
        AcceptedAtUtc = DateTime.UtcNow;
    }
}