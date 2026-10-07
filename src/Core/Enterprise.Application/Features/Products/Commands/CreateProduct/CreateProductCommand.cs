using Enterprise.Application.Common.Models;
using MediatR;

namespace Enterprise.Application.Features.Products.Commands.CreateProduct;

/// <summary>
/// Command to create a new product.
/// </summary>
public record CreateProductCommand(
    string Name,
    string Description,
    string Sku,
    decimal Price,
    string Currency,
    int InitialStock = 0
) : IRequest<Result<Guid>>;
