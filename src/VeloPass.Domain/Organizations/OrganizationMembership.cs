using VeloPass.Domain.Abstractions;

namespace VeloPass.Domain.Organizations;

public sealed class OrganizationMembership : Entity
{
    private OrganizationMembership(
        Guid id,
        Guid organizationId,
        Guid userId,
        OrganizationRole role) : base(id)
    {
        OrganizationId = organizationId;
        UserId = userId;
        Role = role;
    }

    private OrganizationMembership()
    {
    }

    public Guid OrganizationId { get; private set; }
    public Guid UserId { get; private set; }
    public OrganizationRole Role { get; private set; }

    public static OrganizationMembership Create(
        Guid organizationId,
        Guid userId,
        OrganizationRole role)
    {
        return new OrganizationMembership(
            Guid.CreateVersion7(),
            organizationId,
            userId,
            role);
    }

    public void ChangeRole(OrganizationRole newRole)
    {
        Role = newRole;
    }
}