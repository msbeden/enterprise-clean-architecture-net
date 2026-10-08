using Enterprise.Application.Common.Models;
using MediatR;

namespace Enterprise.Application.Features.Orders.Commands.CreateOrder;

public record CreateOrderItemRequest(
    Guid ProductId,
    int Quantity
);

public record CreateOrderCommand(
    Guid UserId,
    string Street,
    string City,
    string State,
    string ZipCode,
    string Country,
    List<CreateOrderItemRequest> Items
) : IRequest<Result<Guid>>;
