using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Invites.Events;
using VeloPass.Domain.Organizations;

namespace VeloPass.Domain.Invites;

public sealed class Invite : Entity
{
    private Invite(Guid id, string email, OrganizationRole role, Guid invitedByUserId, Guid organizationId,
        string token, DateTime expiredAtUtc) : base(id)
    {
        Email = email;
        OrganizationRole = role;
        InvitedByUserId = invitedByUserId;
        OrganizationId = organizationId;
        Token = token;
        ExpiredAtUtc = expiredAtUtc;
    }

    private Invite()
    {
    }

    public string Email { get; private set; } = string.Empty;
    public OrganizationRole OrganizationRole { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid InvitedByUserId { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiredAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? AcceptedAtUtc { get; private set; }
    
    public bool IsExpired(DateTime nowUtc)
    {
        return nowUtc > ExpiredAtUtc;
    }

    public static Invite Create(string email, OrganizationRole role, Guid invitedByUserId, Guid organizationId,
        string token, DateTime expiredAtUtc)
    {
        var invite =
            new Invite(Guid.CreateVersion7(), email, role, invitedByUserId, organizationId, token, expiredAtUtc)
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