namespace Enterprise.Application.Common.Models;

/// <summary>
/// Model representing an audit log entry stored in MongoDB or log destination.
/// </summary>
public class AuditLogEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // e.g. Created, Updated, Deleted
    public string? PerformedBy { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? ChangesJson { get; set; }
}
