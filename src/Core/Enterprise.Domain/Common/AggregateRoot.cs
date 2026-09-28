namespace Enterprise.Domain.Common;

/// <summary>
/// Abstract base class representing an Aggregate Root in Domain-Driven Design.
/// </summary>
/// <typeparam name="TId">Type of aggregate root primary key.</typeparam>
public abstract class AggregateRoot<TId> : BaseAuditableEntity<TId> where TId : notnull
{
    protected AggregateRoot(TId id) : base(id)
    {
    }

    protected AggregateRoot() : base()
    {
    }
}
