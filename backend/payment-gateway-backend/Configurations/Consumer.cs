using payment_gateway_backend.Models;
using payment_gateway_backend.Models.Payment;
using payment_gateway_backend.Models.PaymentTransaction;
using payment_gateway_backend.Services.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace payment_gateway_backend.Configurations;

public class Consumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly IServiceProvider _services;
    private readonly IPaymentSimulator _simulator;
    private readonly string _exchangeName;
    private readonly ILogger<Consumer> _logger;
    public Consumer(
        IServiceProvider services,
        IConfiguration config,
        IPaymentSimulator simulator,
        ILogger<Consumer> logger)
    {
        _services = services;
        _logger = logger;
        var factory = new ConnectionFactory
        {
            HostName = config["RabbitMQ:HostName"],
            UserName = config["RabbitMQ:Username"],
            Password = config["RabbitMQ:Password"],
            Port = 5672
        };
        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        _exchangeName = config["RabbitMQ:ExchangeName"];
        _channel.ExchangeDeclareAsync(exchange: _exchangeName, type: ExchangeType.Fanout).GetAwaiter().GetResult();
        _simulator = simulator;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueName = (await _channel.QueueDeclareAsync()).QueueName;
        await _channel.QueueBindAsync(queue: queueName, exchange: _exchangeName, routingKey: "");

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            using var scope = _services.CreateScope();
            var publisher = scope.ServiceProvider.GetRequiredService<Publisher>();
            var _paymentTransactionRepository = scope.ServiceProvider.GetRequiredService<IPaymentTransactionService>();
            var _eventLogService = scope.ServiceProvider.GetRequiredService<IEventLogService>();

            try
            {
                var routingKey = ea.RoutingKey;
                _logger.LogInformation("Received event with routing key: {RoutingKey}", routingKey);

                // Only process payment initiation events for simulation.
                if (routingKey == "payment.initiated")
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var initiatedEvent = JsonSerializer.Deserialize<PaymentInitiatedEvent>(message);

                    // Optional: check if simulation is necessary by verifying the transaction's current status.
                    var transaction = await _paymentTransactionRepository.GetTransactionByIdAsync(initiatedEvent.TransactionId);
                    if (transaction != null && transaction.Status != "Initiated")
                    {
                        _logger.LogInformation("Skipping simulation for Transaction {TransactionId} as its status is already updated.", initiatedEvent.TransactionId);
                        return;
                    }

                    // Simulate payment processing
                    var simulatedResult = await _simulator.SimulatePaymentAsync(new PaymentRequestDto
                    {
                        Amount = initiatedEvent.Amount,
                        Currency = initiatedEvent.Currency,
                        CustomerEmail = initiatedEvent.CustomerEmail
                    });

                    // Update transaction status in the repository
                    bool updated = await _paymentTransactionRepository.UpdateTransactionStatusAsync(
                                                                    initiatedEvent.TransactionId,
                                                                    new UpdateTransactionStatusDto() { Status = simulatedResult.Status });

                    if (!updated)
                    {
                        _logger.LogWarning("Transaction not found: {TransactionId}", initiatedEvent.TransactionId);
                    }

                    // Publish the status update event
                    await publisher.PublishEventAsync(new PaymentStatusUpdatedEvent
                    {
                        TransactionId = initiatedEvent.TransactionId,
                        Status = simulatedResult.Status,
                        UpdatedAt = DateTime.UtcNow
                    }, "payment.status.updated");

                    // Log the event for monitoring
                    await _eventLogService.AddLogAsync("Payment Status Updated", $"Transaction Id: {initiatedEvent.TransactionId} | Status: {simulatedResult.Status}");
                }
                else
                {
                    // For events that don't require simulation, simply log and skip.
                    _logger.LogInformation("Event with routing key {RoutingKey} does not require simulation. Skipping processing.", routingKey);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment event");
            }
        };

        await _channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);
        await Task.CompletedTask;
    }

}
