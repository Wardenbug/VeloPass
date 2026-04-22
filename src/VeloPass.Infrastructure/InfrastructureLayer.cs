using System.Text;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using VeloPass.Application.Abstractions;
using VeloPass.Domain.Abstractions;
using VeloPass.Domain.Invites;
using VeloPass.Domain.Organizations;
using VeloPass.Domain.Users;
using VeloPass.Infrastructure.Authentication;
using VeloPass.Infrastructure.Data;
using VeloPass.Infrastructure.Email;
using VeloPass.Infrastructure.Invites;
using VeloPass.Infrastructure.Organizations;
using VeloPass.Infrastructure.Outbox;
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


        services.AddHangfire(config =>
            config.UsePostgreSqlStorage(options =>
                options.UseNpgsqlConnection(connectionString)));
        
        services.AddHangfireServer(options => 
            options.SchedulePollingInterval = TimeSpan.FromSeconds(1));
        
        
        services.Configure<GoogleOptions>(
            configuration.GetSection(GoogleOptions.Google));
        services.Configure<EmailOptions>(
            configuration.GetSection(EmailOptions.SectionName));
        
        services.AddScoped<IExternalIdentityTokenValidator, ExternalIdentityTokenValidator>();
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IOrganizationMembersRepository, OrganizationMembersRepository>();
        services.AddScoped<IInviteRepository, InviteRepository>();
        
        services.AddScoped<IUserRegistrationService, UserRegistrationService>();
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<IOutboxProcessor,  OutboxProcessor>();
        
        services.AddTransient<IJwtService, JwtService>();

        services.AddScoped<IUnitOfWork>(serviceProvider =>
            serviceProvider.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}