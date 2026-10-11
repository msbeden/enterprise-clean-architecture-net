using Enterprise.Application.Common.Interfaces;
using Enterprise.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Enterprise.Persistence.Interceptors;

/// <summary>
/// EF Core Interceptor to automatically handle audit timestamps (CreatedUtc, LastModifiedUtc)
/// when saving changes to entities implementing IAuditableEntity.
/// </summary>
public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly IDateTimeService _dateTimeService;

    public AuditableEntityInterceptor(IDateTimeService dateTimeService)
    {
        _dateTimeService = dateTimeService;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedUtc = _dateTimeService.UtcNow;
                entry.Entity.CreatedBy ??= "System";
            }

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                entry.Entity.LastModifiedUtc = _dateTimeService.UtcNow;
                entry.Entity.LastModifiedBy ??= "System";
            }
        }
    }
}

public static class Extensions
{
    public static bool HasChangedOwnedEntities(this Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}
