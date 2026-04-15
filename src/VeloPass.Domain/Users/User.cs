using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Users.Events;

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
        var user = new User(Guid.CreateVersion7(), identityId, name, email);

        user.RaiseDomainEvent(new UserCreatedDomainEvent(email));
        
        return user;
    }
}