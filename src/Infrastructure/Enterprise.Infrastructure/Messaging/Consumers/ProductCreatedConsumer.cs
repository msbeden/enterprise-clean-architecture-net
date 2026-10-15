using Enterprise.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Enterprise.Infrastructure.Messaging.Consumers;

/// <summary>
/// MassTransit Consumer that listens for ProductCreatedEvent messages from RabbitMQ.
/// </summary>
public class ProductCreatedConsumer : IConsumer<ProductCreatedEvent>
{
    private readonly ILogger<ProductCreatedConsumer> _logger;

    public ProductCreatedConsumer(ILogger<ProductCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<ProductCreatedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation(
            "Received ProductCreatedEvent for Product ID '{ProductId}', Name: '{ProductName}', SKU: '{Sku}', Price: '{Price} {Currency}', Initial Stock: {Stock}",
            message.ProductId,
            message.Name,
            message.Sku,
            message.PriceAmount,
            message.Currency,
            message.StockQuantity
        );

        // Additional asynchronous side-effects can be handled here (e.g., sending notification, updating analytics, etc.)

        return Task.CompletedTask;
    }
}
