using VeloPass.Domain.Abstractions;

namespace VeloPass.Domain.Users;

public sealed class User : Entity
{
    private User(Guid id, string identityId) : base(id)
    {
        IdentityId = identityId;
    }

    private User()
    {
    }
    
    public string IdentityId { get; private set; } = String.Empty;

    public static User Create(string identityId)
    {
        return new User(Guid.CreateVersion7(), identityId);
    }
}