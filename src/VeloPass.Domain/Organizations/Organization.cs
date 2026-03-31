using VeloPass.Domain.Abstractions;

namespace VeloPass.Domain.Organizations;

public sealed class Organization : Entity
{
    private Organization(Guid id, string name, string ownerId) : base(id)
    {
        Name = name;
        OwnerId = ownerId;
    }

    private Organization()
    {
    }

    public string Name { get; private set; } = String.Empty;
    public string OwnerId { get; private set; } = String.Empty;


    public static Organization Create(string name, string ownerId)
    {
        return new Organization(Guid.CreateVersion7(),name, ownerId);
    }
}