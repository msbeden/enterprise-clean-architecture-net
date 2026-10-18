using Enterprise.Application.Common.Models;
using Enterprise.Application.Features.Products.Commands.CreateProduct;
using Enterprise.Application.Features.Products.DTOs;
using Enterprise.Application.Features.Products.Queries.GetProductById;
using Enterprise.Application.Features.Products.Queries.GetProductsWithPagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enterprise.WebAPI.Controllers;

/// <summary>
/// API Controller for managing products.
/// Publicly accessible without authentication.
/// </summary>
[AllowAnonymous]
public class ProductsController : ApiControllerBase
{
    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="command">Product creation data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of the created product.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result);
    }

    /// <summary>
    /// Retrieves a product by its unique identifier.
    /// </summary>
    /// <param name="id">Product unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Product details.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Result<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetProductByIdQuery(id), cancellationToken);
        if (result.IsFailure)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Retrieves a paginated list of products with optional search filter.
    /// </summary>
    /// <param name="query">Pagination and search parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of products.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(Result<PaginatedList<ProductDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaginated([FromQuery] GetProductsWithPaginationQuery query, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(query, cancellationToken);
        return Ok(result);
    }
}
