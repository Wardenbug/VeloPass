using Microsoft.EntityFrameworkCore;
using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Users;
using VeloPass.Infrastructure.Data;

namespace VeloPass.Infrastructure.Users;

public sealed class UserRepository(ApplicationDbContext dbContext) : IUserRepository
{
    public void Add(User user)
    {
        dbContext.Add(user);
    }

    public async Task<Result<User>> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Set<User>()
            .Where(user => user.Email == email)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return Result.NotFound<User>("User not found");
        }

        return Result.Ok(user);
    }

    public async Task<Result<User>> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Set<User>()
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id.ToString() == userId, cancellationToken);

        if (user is null)
        {
            return Result.NotFound<User>("User not found");
        }
        
        return Result.Ok(user);
    }
}