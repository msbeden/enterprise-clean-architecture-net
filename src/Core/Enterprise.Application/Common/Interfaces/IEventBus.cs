namespace Enterprise.Application.Common.Interfaces;

/// <summary>
/// Abstraction for asynchronous event messaging (e.g. MassTransit / RabbitMQ).
/// </summary>
public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class;
}
