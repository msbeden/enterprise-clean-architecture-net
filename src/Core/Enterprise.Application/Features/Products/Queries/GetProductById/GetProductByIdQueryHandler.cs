using Enterprise.Application.Common.Interfaces;
using Enterprise.Application.Common.Models;
using Enterprise.Application.Features.Products.DTOs;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Enterprise.Application.Features.Products.Queries.GetProductById;

/// <summary>
/// Handler for GetProductByIdQuery.
/// </summary>
public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProductByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product is null)
        {
            return Result<ProductDto>.Failure($"Product with ID '{request.Id}' was not found.");
        }

        var dto = product.Adapt<ProductDto>();
        return Result<ProductDto>.Success(dto);
    }
}
