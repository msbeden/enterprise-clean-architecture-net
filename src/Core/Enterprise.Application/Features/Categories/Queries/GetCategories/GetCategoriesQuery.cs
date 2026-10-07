using Enterprise.Application.Common.Models;
using Enterprise.Application.Features.Categories.DTOs;
using MediatR;

namespace Enterprise.Application.Features.Categories.Queries.GetCategories;

/// <summary>
/// Query to retrieve all active categories.
/// </summary>
public record GetCategoriesQuery() : IRequest<Result<List<CategoryDto>>>;
