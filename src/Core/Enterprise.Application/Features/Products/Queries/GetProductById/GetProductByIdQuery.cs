using Enterprise.Application.Common.Models;
using Enterprise.Application.Features.Products.DTOs;
using MediatR;

namespace Enterprise.Application.Features.Products.Queries.GetProductById;

/// <summary>
/// Query to retrieve product by ID.
/// </summary>
public record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDto>>;
