using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace BuildingBlocks.Messaging.RabbitMQ
{
    public class EventConsumer : IEventConsumer, IDisposable
    {
        private readonly PersistentConnection _persistentConnection;
        private IChannel? _channel;

        public EventConsumer(PersistentConnection persistentConnection)
        {
            _persistentConnection = persistentConnection;
            _channel = _persistentConnection.CreateChannel();
        }

        public void Dispose()
        {
            _channel?.CloseAsync().GetAwaiter().GetResult();
            _channel?.Dispose();
            _channel = null;
        }

        public void StartConsuming(Action<string> onMessageReceived, ConsumerOptions? options = null)
        {
            options ??= new ConsumerOptions();

            if (_channel is null || _channel.IsClosed)
            {
                _channel?.Dispose();
                _channel = _persistentConnection.CreateChannel();
            }

            _channel.ExchangeDeclareAsync(
                options.ExchangeName,
                options.TypeExchange,
                options.Durable,
                options.AutoDelete,
                options.Arguments).GetAwaiter().GetResult();

            _channel.QueueDeclareAsync(
                options.QueueName,
                options.Durable,
                false,
                options.AutoDelete,
                options.Arguments).GetAwaiter().GetResult();

            _channel.QueueBindAsync(
                options.QueueName,
                options.ExchangeName,
                options.RoutingKey).GetAwaiter().GetResult();

            _channel.BasicQosAsync(0, 1, false).GetAwaiter().GetResult();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (ch, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                onMessageReceived(json);
                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            };

            _channel.BasicConsumeAsync(
                options.QueueName,
                false,
                consumer).GetAwaiter().GetResult();
        }
    }
}