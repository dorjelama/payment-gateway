using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace payment_gateway_backend.Configurations;

public class Publisher : IAsyncDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly string _exchangeName;

    public Publisher(IConfiguration config)
    {
        var factory = new ConnectionFactory
        {
            HostName = config["RabbitMQ:HostName"],
            UserName = config["RabbitMQ:Username"],
            Password = config["RabbitMQ:Password"]
        };
        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        _exchangeName = config["RabbitMQ:ExchangeName"];
        _channel.ExchangeDeclareAsync(exchange: _exchangeName, type: ExchangeType.Fanout).GetAwaiter().GetResult();
    }

    public async Task PublishEventAsync(object eventData, string eventType)
    {
        var message = JsonSerializer.Serialize(eventData);
        var body = Encoding.UTF8.GetBytes(message);
        await _channel.BasicPublishAsync(
            exchange: _exchangeName,
            routingKey: eventType,
            body: body);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
            await _channel.CloseAsync();
        if (_connection is not null)
            await _connection.CloseAsync();
    }
}
