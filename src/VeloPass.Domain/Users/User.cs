using VeloPass.Domain.Abstractions;

namespace VeloPass.Domain.Users;

public sealed class User : Entity
{
    private User(Guid id, string identityId, string name, string email) : base(id)
    {
        IdentityId = identityId;
        Name = name;
        Email = email;
    }

    private User()
    {
    }
    
    public string IdentityId { get; private set; } = String.Empty;
    public string Name { get; private set; } = String.Empty;
    public string Email { get; private set; } = String.Empty;

    public static User Create(string identityId, string name, string email)
    {
        return new User(Guid.CreateVersion7(), identityId, name, email);
    }
}