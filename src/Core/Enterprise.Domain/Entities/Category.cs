using Enterprise.Domain.Common;
using Enterprise.Domain.Events;
using Enterprise.Domain.Exceptions;

namespace Enterprise.Domain.Entities;

public class Category : AggregateRoot<Guid>
{
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<Product> _products = new();
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    private Category() : base() { }

    private Category(Guid id, string name, string description, int displayOrder) : base(id)
    {
        Name = name;
        Description = description;
        DisplayOrder = displayOrder;
        IsActive = true;
    }

    public static Category Create(string name, string description = "", int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Category name cannot be empty.");

        var category = new Category(Guid.NewGuid(), name, description ?? string.Empty, displayOrder);

        category.AddDomainEvent(new CategoryCreatedEvent(category.Id, category.Name, category.DisplayOrder));

        return category;
    }

    public void UpdateDetails(string name, string description, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Category name cannot be empty.");

        Name = name;
        Description = description ?? string.Empty;
        DisplayOrder = displayOrder;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
