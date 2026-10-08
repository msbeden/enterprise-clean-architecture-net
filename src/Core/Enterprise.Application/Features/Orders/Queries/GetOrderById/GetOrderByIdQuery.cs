using Enterprise.Application.Common.Models;
using Enterprise.Application.Features.Orders.DTOs;
using MediatR;

namespace Enterprise.Application.Features.Orders.Queries.GetOrderById;

/// <summary>
/// Query to retrieve order by ID.
/// </summary>
public record GetOrderByIdQuery(Guid Id) : IRequest<Result<OrderDto>>;
