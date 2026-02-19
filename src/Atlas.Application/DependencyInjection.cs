using Atlas.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Atlas.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<AuthService>();
        services.AddScoped<ProjectsService>();
        return services;
    }
}
