using VeloPass.Domain.Invites;
using VeloPass.Infrastructure.Data;

namespace VeloPass.Infrastructure.Invites;

internal sealed class InviteRepository(ApplicationDbContext dbContext) : IInviteRepository
{
    public void Add(Invite invite)
    {
        dbContext.Add(invite);
    }
}