using Atlas.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Atlas.Tests;

public sealed class TestAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Replace SQL Server with InMemory provider for tests
            var dbOptions = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AtlasDbContext>));
            if (dbOptions is not null) services.Remove(dbOptions);

            services.AddDbContext<AtlasDbContext>(o => o.UseInMemoryDatabase("atlas-tests"));

            // Build provider and ensure created
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AtlasDbContext>();
            db.Database.EnsureCreated();
        });
    }
}
