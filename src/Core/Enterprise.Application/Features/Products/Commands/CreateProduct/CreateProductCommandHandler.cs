using Enterprise.Application.Common.Interfaces;
using Enterprise.Application.Common.Models;
using Enterprise.Domain.Entities;
using Enterprise.Domain.ValueObjects;
using MediatR;

namespace Enterprise.Application.Features.Products.Commands.CreateProduct;

/// <summary>
/// Handler for CreateProductCommand.
/// </summary>
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var sku = SkuCode.Create(request.Sku);
        var price = Money.Create(request.Price, request.Currency);

        var product = Product.Create(
            request.Name,
            request.Description,
            sku,
            price,
            request.InitialStock
        );

        await _context.Products.AddAsync(product, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(product.Id);
    }
}
