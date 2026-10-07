using Enterprise.Application.Common.Models;
using Enterprise.Application.Features.Products.DTOs;
using MediatR;

namespace Enterprise.Application.Features.Products.Queries.GetProductsWithPagination;

/// <summary>
/// Query to retrieve paginated products with optional search filter.
/// </summary>
public record GetProductsWithPaginationQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null
) : IRequest<Result<PaginatedList<ProductDto>>>;
