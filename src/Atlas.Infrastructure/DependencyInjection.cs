using Atlas.Application.Abstractions.Persistence;
using Atlas.Application.Abstractions.Security;
using Atlas.Application.Abstractions.Time;
using Atlas.Infrastructure.Persistence;
using Atlas.Infrastructure.Persistence.Repositories;
using Atlas.Infrastructure.Security;
using Atlas.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Atlas.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AtlasDbContext>(o =>
        {
            var cs = config.GetConnectionString("Default");
            o.UseSqlServer(cs, sql => sql.EnableRetryOnFailure());
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<IClock, SystemClock>();

        var jwtOptions = new JwtOptions();
        config.GetSection("Jwt").Bind(jwtOptions);
        services.AddSingleton(jwtOptions);

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        return services;
    }
}
