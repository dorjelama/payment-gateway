using payment_gateway_backend.Models.Payment;
using payment_gateway_backend.Services.Interfaces;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace payment_gateway_backend.Configurations
{
    public class FailureConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly ILogger<FailureConsumer> _logger;
        private readonly IServiceProvider _services;

        public FailureConsumer(
            IConfiguration config,
            ILogger<FailureConsumer> logger,
            IServiceProvider services)
        {

            var factory = new ConnectionFactory
            {
                HostName = config["RabbitMQ:HostName"],
                UserName = config["RabbitMQ:Username"],
                Password = config["RabbitMQ:Password"]
            };
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

            // Declare exchange and queue
            _channel.ExchangeDeclareAsync(exchange: "payment.exchange", type: ExchangeType.Topic).GetAwaiter().GetResult();
            _channel.QueueDeclareAsync(queue: "payment.failures", durable: true, exclusive: false, autoDelete: false, arguments: null).GetAwaiter().GetResult();
            _channel.QueueBindAsync(queue: "payment.failures", exchange: "payment.exchange", routingKey: "payment.failure").GetAwaiter().GetResult();

            _logger = logger;
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                using var scope = _services.CreateScope();
                var publisher = scope.ServiceProvider.GetRequiredService<Publisher>();
                var _paymentTransactionRepository = scope.ServiceProvider.GetRequiredService<IPaymentTransactionService>();
                var _eventLogService = scope.ServiceProvider.GetRequiredService<IEventLogService>();
                var _paymentProcessService = scope.ServiceProvider.GetRequiredService<IPaymentProcessService>();
                var _notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var failureEvent = JsonSerializer.Deserialize<PaymentFailureEvent>(message);

                    if (failureEvent == null)
                    {
                        _logger.LogError("Failed to deserialize failure event.");
                        return;
                    }

                    _logger.LogError("Failure event received: Transaction ID: {TransactionId}, Error: {ErrorMessage}",
                        failureEvent.TransactionId, failureEvent.ErrorMessage);

                    // Log the failure in the database
                    await _eventLogService.AddLogAsync("Payment Failure",
                        $"Transaction ID: {failureEvent.TransactionId}, Error: {failureEvent.ErrorMessage}");

                    // Notify administrators (e.g., via email or SMS)
                    await NotifyAdministratorsAsync(failureEvent);

                    // Optionally, retry the failed operation
                    await RetryFailedOperationAsync(failureEvent);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing failure event");
                }
            };

            await _channel.BasicConsumeAsync(queue: "payment.failures", autoAck: true, consumer: consumer);

            await Task.CompletedTask;
        }

        private async Task NotifyAdministratorsAsync(PaymentFailureEvent failureEvent)
        {
            using var scope = _services.CreateScope();
            var _notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            // Example: Send an email or SMS notification
            await _notificationService.SendNotificationAsync(
                subject: "Payment Processing Failure",
                message: $"A failure occurred for Transaction ID: {failureEvent.TransactionId}. Error: {failureEvent.ErrorMessage}"
            );
        }

        private async Task RetryFailedOperationAsync(PaymentFailureEvent failureEvent)
        {
            var retryPolicy = Policy
                .Handle<Exception>()
                .RetryAsync(3, (exception, retryCount) =>
                {
                    _logger.LogWarning(exception, "Retry attempt {RetryCount} for Transaction ID: {TransactionId}",
                        retryCount, failureEvent.TransactionId);
                });

            await retryPolicy.ExecuteAsync(async () =>
            {
                using var scope = _services.CreateScope();
                var _paymentProcessService = scope.ServiceProvider.GetRequiredService<IPaymentProcessService>();
                var _paymentTransactionService = scope.ServiceProvider.GetRequiredService<IPaymentTransactionService>();

                var transaction = await _paymentTransactionService.GetTransactionByIdAsync(failureEvent.TransactionId);

                await _paymentProcessService.RetryProcessPaymentAsync(new PaymentRequestDto
                {
                    Amount = transaction.Amount,
                    Currency = transaction.Currency,
                    CustomerEmail = "",
                    CardOrAccountNumber = "",
                    CVV = 0,
                    CustomerName = "",
                    PaymentMethod = transaction.PaymentMethod
                }, failureEvent.TransactionId);
            });
        }

        public async ValueTask Dispose()
        {
            if (_channel is not null)
                await _channel.CloseAsync();
            if (_connection is not null)
                await _connection.CloseAsync();
            base.Dispose();
        }
    }
}