using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VeloPass.Application.Users.RegisterUser;
using Wolverine;

namespace VeloPass.Application;

public static class ApplicationLayer
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services, IHostBuilder hostBuilder)
    {
        hostBuilder.UseWolverine();
        
        return services;
    }
}