using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VeloPass.Infrastructure.Data;

namespace VeloPass.Infrastructure;

public static class InfrastructureLayer
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Database") ??
                                  throw new ArgumentNullException(nameof(configuration));

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options
                .UseNpgsql(
                    connectionString,
                    options => options
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemes.Public)
                )
                .UseSnakeCaseNamingConvention();
        });

        services.AddDbContext<ApplicationIdentityDbContext>(options =>
        {
            options
                .UseNpgsql(
                    connectionString,
                    options => options
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemes.Identity)
                )
                .UseSnakeCaseNamingConvention();
        });

        services.AddIdentityCore<IdentityUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationIdentityDbContext>();


        services.AddAuthentication();
        services.AddAuthorization();
        
        return services;
    }
}