namespace VeloPass.Domain.Users;

public interface IUserRepository
{
    void Add(User user);
    
    Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
}