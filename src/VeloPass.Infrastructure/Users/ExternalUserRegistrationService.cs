using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using VeloPass.Application.Abstractions;
using VeloPass.Application.Authentication;
using VeloPass.Application.Identity;
using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Users;
using VeloPass.Infrastructure.Authentication;
using VeloPass.Infrastructure.Data;

namespace VeloPass.Infrastructure.Users;

public sealed class ExternalUserRegistrationService(
    ApplicationDbContext applicationDbContext,
    ApplicationIdentityDbContext applicationIdentityDbContext,
    IUserRepository userRepository,
    UserManager<IdentityUser> userManager,
    IJwtService jwtService,
    IOptions<JwtAuthOptions> options) : IExternalUserRegistrationService
{
    public async Task<Result<AccessTokenDto>> RegisterAsync(
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
}