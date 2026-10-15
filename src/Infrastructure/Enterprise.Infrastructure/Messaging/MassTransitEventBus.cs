using Enterprise.Application.Common.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Enterprise.Infrastructure.Messaging;

/// <summary>
/// Implementation of IEventBus abstraction using MassTransit IPublishEndpoint.
/// </summary>
public class MassTransitEventBus : IEventBus
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<MassTransitEventBus> _logger;

    public MassTransitEventBus(IPublishEndpoint publishEndpoint, ILogger<MassTransitEventBus> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        ArgumentNullException.ThrowIfNull(@event);

        _logger.LogInformation("Publishing event {EventType} via MassTransit EventBus.", typeof(TEvent).Name);
        await _publishEndpoint.Publish(@event, cancellationToken);
    }
}
