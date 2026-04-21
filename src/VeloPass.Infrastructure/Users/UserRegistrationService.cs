using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using VeloPass.Application.Abstractions;
using VeloPass.Application.Authentication;
using VeloPass.Application.Identity;
using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Invites;
using VeloPass.Domain.Organizations;
using VeloPass.Domain.Users;
using VeloPass.Infrastructure.Authentication;
using VeloPass.Infrastructure.Data;

namespace VeloPass.Infrastructure.Users;

public sealed class UserRegistrationService(
    ApplicationDbContext applicationDbContext,
    ApplicationIdentityDbContext applicationIdentityDbContext,
    IInviteRepository inviteRepository,
    IUserRepository userRepository,
    UserManager<IdentityUser> userManager,
    IOrganizationRepository organizationRepository,
    IJwtService jwtService,
    IOptions<JwtAuthOptions> options) : IUserRegistrationService
{
    public async Task<Result<AccessTokenDto>> RegisterByExternalProviderAsync(
        ValidatedExternalIdentity externalIdentity, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(externalIdentity);
        
        using IDbContextTransaction transaction = await applicationIdentityDbContext.Database.BeginTransactionAsync(cancellationToken);
        applicationDbContext.Database.SetDbConnection(applicationIdentityDbContext.Database.GetDbConnection());
        await applicationDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);
        
        var user = await userRepository.FindByEmailAsync(externalIdentity.Email, cancellationToken);
        
        if (user.IsSuccess)
        {
            return Result.Invalid<AccessTokenDto>("Email is already registered");
        }

        var identityUser = new IdentityUser
        {
            Email = externalIdentity.Email,
            UserName = externalIdentity.Email,
            EmailConfirmed = externalIdentity.EmailVerified,
        };

        var identityResult = await userManager.CreateAsync(identityUser);
        
        ArgumentNullException.ThrowIfNull(identityResult);
        
        var newUser = User.Create(identityUser.Id, externalIdentity.Name, externalIdentity.Email);
        
        userRepository.Add(newUser);
        
        var token = jwtService.GenerateJwtToken(new TokenRequest(newUser.Id.ToString()));

        var refreshToken = new RefreshTokenEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = identityUser.Id,
            Token = token.RefreshToken,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(options.Value.RefreshTokenExpirationInMinutes),
        };
        
        applicationIdentityDbContext.Add(refreshToken);
        
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        await applicationIdentityDbContext.SaveChangesAsync(cancellationToken);
        
        await transaction.CommitAsync(cancellationToken);
        
        return Result.Ok(new AccessTokenDto(token.AccessToken, token.RefreshToken));
    }

    public async Task<Result<AccessTokenDto>> RegisterByInviteAsync(string token,
        CancellationToken cancellationToken = default)
    {
        using IDbContextTransaction transaction = await applicationIdentityDbContext.Database.BeginTransactionAsync(cancellationToken);
        applicationDbContext.Database.SetDbConnection(applicationIdentityDbContext.Database.GetDbConnection());
        await applicationDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);

        var invite = await inviteRepository.GetByToken(token, cancellationToken);

        if (!invite.IsSuccess)
        {
            return Result.NotFound<AccessTokenDto>("Invite not found");
        }

        if (invite.Value.IsExpired(DateTime.UtcNow))
        {
            return Result.Invalid<AccessTokenDto>("Invite is expired");
        }
        
        var organization = await organizationRepository.FindByIdAsync(invite.Value.OrganizationId, cancellationToken);

        if (!organization.IsSuccess)
        {
            return Result.NotFound<AccessTokenDto>("Invite is invalid");
        }
        
        invite.Value.Accept();
        
        var user = await userRepository.FindByEmailAsync(invite.Value.Email, cancellationToken);
        
        if (user.IsSuccess)
        {
            return Result.Invalid<AccessTokenDto>("Email is already registered");
        }
        
        var identityUser = new IdentityUser
        {
            Email = invite.Value.Email,
            UserName = invite.Value.Email
        };
        
        var identityResult = await userManager.CreateAsync(identityUser);
        
        ArgumentNullException.ThrowIfNull(identityResult);

        if (!identityResult.Succeeded)
        {
            return Result.Invalid<AccessTokenDto>("Invite is invalid");
        }
        
        var newUser = User.Create(identityUser.Id, invite.Value.Email, invite.Value.Email);
        
        userRepository.Add(newUser);

        organization.Value.AddMember(newUser.Id, invite.Value.OrganizationRole);
        
        var jwtToken = jwtService.GenerateJwtToken(new TokenRequest(newUser.Id.ToString()));

        var refreshToken = new RefreshTokenEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = identityUser.Id,
            Token = jwtToken.RefreshToken,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(options.Value.RefreshTokenExpirationInMinutes),
        };
        
        applicationIdentityDbContext.Add(refreshToken);
        
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        await applicationIdentityDbContext.SaveChangesAsync(cancellationToken);
        
        await transaction.CommitAsync(cancellationToken);
        
        return Result.Ok(new AccessTokenDto(jwtToken.AccessToken, jwtToken.RefreshToken));
    }
}