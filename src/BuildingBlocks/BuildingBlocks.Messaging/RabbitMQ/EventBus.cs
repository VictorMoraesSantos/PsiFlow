using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public class EventBus : IEventBus
{
    private readonly PersistentConnection _conn;

    public EventBus(PersistentConnection conn)
    {
        _conn = conn;
    }

    public void PublishAsync<TEvent>(TEvent @event, PublishOptions? opts = null) where TEvent : IntegrationEvent
    {
        opts ??= new PublishOptions();

        using var channel = _conn.CreateChannel();

        channel.ExchangeDeclareAsync(
            opts.ExchangeName,
            opts.TypeExchange,
            opts.Durable,
            opts.AutoDelete,
            opts.Arguments).GetAwaiter().GetResult();

        var json = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(json);

        var props = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent,
            MessageId = @event.Id.ToString()
        };

        channel.BasicPublishAsync(
            opts.ExchangeName,
            opts.RoutingKey,
            false,
            props,
            body).GetAwaiter().GetResult();
    }
}