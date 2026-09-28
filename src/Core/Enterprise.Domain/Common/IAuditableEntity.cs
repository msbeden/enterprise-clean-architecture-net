namespace Enterprise.Domain.Common;

/// <summary>
/// Interface for entities that require audit trail tracking (Created and Modified timestamps & users).
/// </summary>
public interface IAuditableEntity
{
    DateTime CreatedUtc { get; set; }
    string? CreatedBy { get; set; }
    DateTime? LastModifiedUtc { get; set; }
    string? LastModifiedBy { get; set; }
}
