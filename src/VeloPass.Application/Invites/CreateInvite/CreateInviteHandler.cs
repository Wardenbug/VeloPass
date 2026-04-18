using System.Buffers.Text;
using System.Security.Cryptography;
using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Invites;
using VeloPass.Domain.Organizations;
using VeloPass.Domain.Users;

namespace VeloPass.Application.Invites.CreateInvite;

public class CreateInviteHandler(
    IUnitOfWork unitOfWork,
    IOrganizationRepository organizationRepository,
    IInviteRepository inviteRepository,
    IUserRepository userRepository)
{
    public async Task<Result<Invite>> Handle(CreateInviteCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var member = await organizationRepository
            .FindMembershipByUserIdAsync(command.InvitedByUserId, command.OrganizationId, cancellationToken);
            
        if (!member.IsSuccess)
        {
            return Result.NotFound<Invite>("Member not found");
        }

        if (member.Value.Role != OrganizationRole.Admin && member.Value.Role != OrganizationRole.Owner)
        {
            return Result.Invalid<Invite>("Invite role not allowed");
        }
        
        var user = await userRepository.FindByEmailAsync(command.Email, cancellationToken);

        if (user.IsSuccess)
        {
            return Result.Invalid<Invite>("User already registered");
        }
        
        var userInOrganization =
            await organizationRepository.FindMembershipByEmailAsync(command.Email, command.OrganizationId,
                cancellationToken);

        if (userInOrganization.IsSuccess)
        {
            return Result.Invalid<Invite>("User is already member of organization");
        }
        
        byte[] randomBytes = RandomNumberGenerator.GetBytes(16);
        
        var invite = Invite.Create(
            command.Email,
            command.Role, 
            command.InvitedByUserId, 
            command.OrganizationId,
            Base64Url.EncodeToString(randomBytes),
            DateTime.UtcNow.AddDays(7));
        
        inviteRepository.Add(invite);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Ok(invite);
    }
}