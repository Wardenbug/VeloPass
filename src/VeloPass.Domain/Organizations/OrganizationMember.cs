using VeloPass.Domain.Abstractions;

namespace VeloPass.Domain.Organizations;

public sealed class OrganizationMember : Entity
{
    private OrganizationMember(
        Guid id,
        Guid organizationId,
        Guid userId,
        OrganizationRole role) : base(id)
    {
        OrganizationId = organizationId;
        UserId = userId;
        Role = role;
    }

    private OrganizationMember()
    {
    }

    public Guid OrganizationId { get; private set; }
    public Guid UserId { get; private set; }
    public OrganizationRole Role { get; private set; }

    public static OrganizationMember Create(
        Guid organizationId,
        Guid userId,
        OrganizationRole role)
    {
        return new OrganizationMember(
            Guid.CreateVersion7(),
            organizationId,
            userId,
            role);
    }

    public void ChangeRole(OrganizationRole newRole)
    {
        Role = newRole;
    }
    
    public bool CanManageMembers() => Role.IsPrivileged();
}