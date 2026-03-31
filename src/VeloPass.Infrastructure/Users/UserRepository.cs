using Microsoft.EntityFrameworkCore;
using VeloPass.Domain.Users;
using VeloPass.Infrastructure.Data;

namespace VeloPass.Infrastructure.Users;

public class UserRepository(ApplicationDbContext dbContext) : IUserRepository
{
    public void Add(User user)
    {
        dbContext.Add(user);
    }

    public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Set<User>()
            .Where(user => user.Email == email)
            .FirstOrDefaultAsync(cancellationToken);

        return user;
    }
}