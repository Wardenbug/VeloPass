using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using VeloPass.Application.Abstractions;
using VeloPass.Application.Identity;
using VeloPass.Domain.Users;
using VeloPass.Infrastructure.Data;

namespace VeloPass.Infrastructure.Users;

public sealed class ExternalUserRegistrationService(
    ApplicationDbContext applicationDbContext,
    ApplicationIdentityDbContext applicationIdentityDbContext,
    IUserRepository userRepository,
    UserManager<IdentityUser> userManager) : IExternalUserRegistrationService
{
    public async Task<User> RegisterAsync(
        ValidatedExternalIdentity externalIdentity, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(externalIdentity);
        
        using IDbContextTransaction transaction = await applicationIdentityDbContext.Database.BeginTransactionAsync(cancellationToken);
        applicationDbContext.Database.SetDbConnection(applicationIdentityDbContext.Database.GetDbConnection());
        await applicationDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);
        
        var user = await userRepository.FindByEmailAsync(externalIdentity.Email, cancellationToken);

        if (user is not null)
        {
            return user;
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
        
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        
        await transaction.CommitAsync(cancellationToken);
        
        return newUser;
    }
}