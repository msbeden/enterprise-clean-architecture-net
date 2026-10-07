using Enterprise.Application.Common.Interfaces;
using Enterprise.Application.Common.Models;
using Enterprise.Application.Features.Categories.DTOs;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Enterprise.Application.Features.Categories.Queries.GetCategories;

/// <summary>
/// Handler for GetCategoriesQuery.
/// </summary>
public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, Result<List<CategoryDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetCategoriesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ProjectToType<CategoryDto>()
            .ToListAsync(cancellationToken);

        return Result<List<CategoryDto>>.Success(categories);
    }
}
