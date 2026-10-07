using Enterprise.Application.Common.Models;
using MediatR;

namespace Enterprise.Application.Features.Categories.Commands.CreateCategory;

/// <summary>
/// Command to create a new category.
/// </summary>
public record CreateCategoryCommand(
    string Name,
    string Description = "",
    int DisplayOrder = 0
) : IRequest<Result<Guid>>;
