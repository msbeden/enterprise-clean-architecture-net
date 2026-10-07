using Enterprise.Application.Common.Interfaces;
using Enterprise.Application.Common.Models;
using Enterprise.Application.Features.Products.DTOs;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Enterprise.Application.Features.Products.Queries.GetProductsWithPagination;

/// <summary>
/// Handler for GetProductsWithPaginationQuery.
/// </summary>
public class GetProductsWithPaginationQueryHandler : IRequestHandler<GetProductsWithPaginationQuery, Result<PaginatedList<ProductDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetProductsWithPaginationQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<ProductDto>>> Handle(GetProductsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.Trim();
            query = query.Where(p =>
                p.Name.Contains(search) ||
                p.Description.Contains(search) ||
                p.Sku.Value.Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var products = await query
            .OrderBy(p => p.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = products.Adapt<List<ProductDto>>();
        var paginatedList = new PaginatedList<ProductDto>(dtos, totalCount, request.PageNumber, request.PageSize);

        return Result<PaginatedList<ProductDto>>.Success(paginatedList);
    }
}
