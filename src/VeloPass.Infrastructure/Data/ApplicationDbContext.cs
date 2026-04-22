using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VeloPass.Domain.Abstractions;
using VeloPass.Infrastructure.Configurations;
using VeloPass.Infrastructure.Outbox;
using Wolverine;

namespace VeloPass.Infrastructure.Data;

public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var domainEvents = ChangeTracker.Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var domainEvents = entity.GetDomainEvents();
                
                entity.ClearDomainEvents();
                
                return domainEvents;
            })
            .ToList();
 
            foreach (var domainEvent in domainEvents)
            {
                Add(new OutboxMessage
                {
                    Id = Guid.CreateVersion7(),
                    Type = domainEvent.GetType().Name,
                    Content = JsonConvert.SerializeObject(domainEvent, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    }),
                    OccurredOnUtc =  DateTime.UtcNow
                });
            }
        
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        
        modelBuilder.HasDefaultSchema(Schemes.Public);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new OrganizationConfiguration());
        modelBuilder.ApplyConfiguration(new OrganizationMembersConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InviteConfiguration());
    }
}