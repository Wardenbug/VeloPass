using VeloPass.Domain.Abstractions;

namespace VeloPass.Domain.Invites;

public interface IInviteRepository
{
    void Add(Invite invite);
    
    void Remove(Invite invite);
    
    Task<Result<Invite>> GetByToken(string token, CancellationToken cancellationToken = default);
}