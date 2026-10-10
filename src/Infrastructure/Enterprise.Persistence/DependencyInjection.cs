using Enterprise.Application.Common.Interfaces;
using Enterprise.Persistence.Context;
using Enterprise.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Persistence;

/// <summary>
/// Extension methods to register Persistence layer services in Dependency Injection container.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditableEntityInterceptor>();

        var databaseProvider = configuration["DatabaseProvider"]
            ?? (string.Equals(configuration["UseInMemoryDatabase"], "true", StringComparison.OrdinalIgnoreCase) ? "InMemory" : "SqlServer");

        if (string.Equals(databaseProvider, "InMemory", StringComparison.OrdinalIgnoreCase))
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("EnterpriseCleanDbTest");
                options.AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>());
            });
        }
        else if (string.Equals(databaseProvider, "PostgreSQL", StringComparison.OrdinalIgnoreCase) || string.Equals(databaseProvider, "Npgsql", StringComparison.OrdinalIgnoreCase))
        {
            var connectionString = configuration.GetConnectionString("PostgresConnection")
                ?? configuration.GetConnectionString("DefaultConnection")
                ?? "Host=localhost;Database=EnterpriseCleanDb;Username=postgres;Password=postgres";

            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseNpgsql(connectionString, builder =>
                {
                    builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                });
                options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
                options.AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>());
            });
        }
        else
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Server=(localdb)\\mssqllocaldb;Database=EnterpriseCleanDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseSqlServer(connectionString, builder =>
                {
                    builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                });
                options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
                options.AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>());
            });
        }

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}
