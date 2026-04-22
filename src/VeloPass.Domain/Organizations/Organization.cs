using VeloPass.Domain.Abstractions;

namespace VeloPass.Domain.Organizations;

public sealed class Organization : Entity
{
    private readonly List<OrganizationMember> _members = [];

    private Organization(Guid id, string name) : base(id)
    {
        Name = name;
    }

    private Organization()
    {
    }

    public string Name { get; private set; } = string.Empty;

    public IReadOnlyCollection<OrganizationMember> Members => _members;

    public static Organization Create(string name, Guid ownerUserId)
    {
        var org = new Organization(Guid.CreateVersion7(), name);
        
        org._members.Add(OrganizationMember.Create(org.Id, ownerUserId, OrganizationRole.Owner));
        
        return org;
    }
    
    public Result<bool> AddMember(Guid userId, OrganizationRole role)
    {
        if (_members.Any(m => m.UserId == userId))
        {
            return Result.Invalid<bool>("User is already a member.");
        }

        _members.Add(OrganizationMember.Create(Id, userId, role));
        
        return Result.Ok(true);
    }

    public Result<bool> ChangeMemberRole(Guid actorUserId, Guid targetUserId, OrganizationRole newRole)
    {
        var actor = _members.FirstOrDefault(m => m.UserId == actorUserId);
        
        if (actor is null)
        {
            return Result.Invalid<bool>("You don’t have access to this organization");
        }
        
        if (!actor.CanManageMembers())
        {
            return Result.Invalid<bool>("You don’t have permission to manage members");
        }
        
        if (newRole == OrganizationRole.Owner)
        {
            return Result.Invalid<bool>("You cannot change the member's role");
        }
        
        var target = _members.FirstOrDefault(m => m.UserId == targetUserId);
        
        if (target is null)
        {
            return Result.Invalid<bool>("Member not found");
        }
        
        if (target.Role == OrganizationRole.Owner)
        {
            return Result.Invalid<bool>("You cannot change the member's role");
        }
        
        target.ChangeRole(newRole);
        
        return Result.Ok(true);
    }
    public Result<bool> RemoveMember(Guid actorUserId, Guid targetUserId)
    {
        var actor = _members.FirstOrDefault(m => m.UserId == actorUserId);
        
        if (actor is null)
        {
            return Result.Invalid<bool>("You don’t have access to this organization");
        }
        
        if (!actor.CanManageMembers())
        {
            return Result.Invalid<bool>("You don’t have permission to manage members");
        }
        
        var target = _members.FirstOrDefault(m => m.UserId == targetUserId);
        
        if (target is null)
        {
            return Result.Invalid<bool>("Member could not be removed");
        }
        
        if (target.Role == OrganizationRole.Owner)
        {
            return Result.Invalid<bool>("Member could not be removed");
        }
        
        _members.Remove(target);
        
        return Result.Ok(true);
    }
}