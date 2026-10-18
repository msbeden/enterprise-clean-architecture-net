using Enterprise.Application.Common.Models;
using Enterprise.Application.Features.Orders.Commands.CreateOrder;
using Enterprise.Application.Features.Orders.DTOs;
using Enterprise.Application.Features.Orders.Queries.GetOrderById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enterprise.WebAPI.Controllers;

/// <summary>
/// API Controller for managing orders.
/// Requires authentication (session/token).
/// </summary>
[Authorize]
public class OrdersController : ApiControllerBase
{
    /// <summary>
    /// Creates a new order.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result);
    }

    /// <summary>
    /// Retrieves an order by its unique identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Result<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetOrderByIdQuery(id), cancellationToken);
        if (result.IsFailure)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}
