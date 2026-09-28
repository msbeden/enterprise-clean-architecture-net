namespace Enterprise.Domain.Common;

/// <summary>
/// Abstract base class for entities requiring generic identity and audit information.
/// </summary>
/// <typeparam name="TId">Type of entity primary key.</typeparam>
public abstract class BaseAuditableEntity<TId> : BaseEntity<TId>, IAuditableEntity where TId : notnull
{
    public DateTime CreatedUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModifiedUtc { get; set; }
    public string? LastModifiedBy { get; set; }

    protected BaseAuditableEntity(TId id) : base(id)
    {
    }

    protected BaseAuditableEntity() : base()
    {
    }
}
