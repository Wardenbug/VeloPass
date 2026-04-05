using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using VeloPass.Application.Abstractions;
using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Organizations;
using VeloPass.Domain.Users;
using VeloPass.Infrastructure.Authentication;
using VeloPass.Infrastructure.Data;
using VeloPass.Infrastructure.Organizations;
using VeloPass.Infrastructure.Users;

namespace VeloPass.Infrastructure;

public static class InfrastructureLayer
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
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

        services.Configure<JwtAuthOptions>(configuration.GetSection("Jwt"));

        var jwtAuthOptions = configuration.GetSection("Jwt").Get<JwtAuthOptions>();
        ArgumentNullException.ThrowIfNull(jwtAuthOptions);
        
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtAuthOptions.Issuer,
                    ValidAudience = jwtAuthOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuthOptions.Key)),
                };
            });
        services.AddAuthorization();


   
        services.Configure<GoogleOptions>(
            configuration.GetSection(GoogleOptions.Google));
        
        services.AddScoped<IExternalIdentityTokenValidator, ExternalIdentityTokenValidator>();
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IOrganizationMembershipRepository, OrganizationMembershipRepository>();
        
        services.AddScoped<IExternalUserRegistrationService, ExternalUserRegistrationService>();
        services.AddTransient<IJwtService, JwtService>();

        services.AddScoped<IUnitOfWork>(serviceProvider =>
            serviceProvider.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}