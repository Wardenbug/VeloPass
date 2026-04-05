using VeloPass.Domain.Abstractions;

namespace VeloPass.Domain.Users;

public interface IUserRepository
{
    void Add(User user);
    
    Task<Result<User>> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
    
    Task<Result<User>> FindByIdAsync(string userId, CancellationToken cancellationToken = default);
}