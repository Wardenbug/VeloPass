using Microsoft.EntityFrameworkCore;
using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Invites;
using VeloPass.Infrastructure.Data;

namespace VeloPass.Infrastructure.Invites;

internal sealed class InviteRepository(ApplicationDbContext dbContext) : IInviteRepository
{
    public void Add(Invite invite)
    {
        dbContext.Add(invite);
    }

    public void Remove(Invite invite)
    {
        dbContext.Remove(invite);
    }

    public async Task<Result<Invite>>GetByToken(string token, CancellationToken cancellationToken = default)
    {
        var invite = await dbContext.Set<Invite>()
            .Where(i => i.Token == token && i.AcceptedAtUtc == null)
            .FirstOrDefaultAsync(cancellationToken);

        if (invite is null)
        {
            return Result.NotFound<Invite>("Invite not found");
        }
        
        return Result.Ok(invite);
    }
}