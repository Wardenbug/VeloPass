using Microsoft.EntityFrameworkCore;
using VeloPass.Infrastructure.Configurations;

namespace VeloPass.Infrastructure.Data;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemes.Public);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}