using VeloPass.Domain.Abstractions;

namespace VeloPass.Domain.Organizations;

public sealed class Organization : Entity
{
    private readonly List<OrganizationMembership> _memberships = [];

    private Organization(Guid id, string name) : base(id)
    {
        Name = name;
    }

    private Organization()
    {
    }

    public string Name { get; private set; } = string.Empty;

    public IReadOnlyCollection<OrganizationMembership> Memberships => _memberships;

    public static Organization Create(string name, Guid ownerUserId)
    {
        var org = new Organization(Guid.CreateVersion7(), name);
        
        org._memberships.Add(OrganizationMembership.Create(org.Id, ownerUserId, OrganizationRole.Owner));
        
        return org;
    }
    
    public Result<bool> AddMember(Guid userId, OrganizationRole role)
    {
        if (_memberships.Any(m => m.UserId == userId))
        {
            return Result.Invalid<bool>("User is already a member.");
        }

        _memberships.Add(OrganizationMembership.Create(Id, userId, role));
        
        return Result.Ok(true);
    }
}