using Enterprise.Application.Common.Interfaces;
using Enterprise.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Enterprise.Persistence.Context;

/// <summary>
/// Design-time Factory for ApplicationDbContext to enable EF Core CLI tools (migrations).
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\mssqllocaldb;Database=EnterpriseCleanDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True",
            b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

        var dateTimeService = new DesignTimeDateTimeService();
        var interceptor = new AuditableEntityInterceptor(dateTimeService);

        return new ApplicationDbContext(optionsBuilder.Options, interceptor);
    }

    private class DesignTimeDateTimeService : IDateTimeService
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
