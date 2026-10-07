namespace Enterprise.Application.Features.Categories.DTOs;

/// <summary>
/// Data Transfer Object for Category entity.
/// </summary>
public record CategoryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string Description { get; init; } = default!;
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime? LastModifiedUtc { get; init; }
}
