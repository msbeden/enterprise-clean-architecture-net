using Enterprise.Application.Common.Models;

namespace Enterprise.Application.Common.Interfaces;

/// <summary>
/// Service interface for audit logging (e.g. via MongoDB).
/// </summary>
public interface IAuditLogService
{
    Task LogAsync(AuditLogEntry entry, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AuditLogEntry>> GetLogsForEntityAsync(string entityName, string entityId, CancellationToken cancellationToken = default);
}
