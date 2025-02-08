using payment_gateway_backend.Entities;
using payment_gateway_backend.Models.Payment;
using payment_gateway_backend.Services.Interfaces;

namespace payment_gateway_backend.Services.Implementations;

public class PaymentSimulatorService : IPaymentSimulator
{
    private readonly ILogger<PaymentSimulatorService> _logger;
    private readonly Random _random = new();

    public PaymentSimulatorService(ILogger<PaymentSimulatorService> logger)
    {
        _logger = logger;
    }

    public async Task<PaymentTransaction> SimulatePaymentAsync(PaymentRequestDto request)
    {
        try
        {
            Random random = new Random();
            int delay = random.Next(3000, 10001);
            await Task.Delay(delay);

            string[] statuses = { "Success", "Pending", "Failed" };
            string status = statuses[_random.Next(statuses.Length)];

            _logger.LogInformation("Simulated payment for {Email} - Status: {Status}", request.CustomerEmail, status);

            return new PaymentTransaction
            {
                //Id = Guid.NewGuid(),
                TransactionId = Guid.NewGuid(),
                Amount = request.Amount,
                Currency = request.Currency,
                //CustomerName = request.CustomerName, get from user
                //CustomerEmail = request.CustomerEmail,
                PaymentMethod = request.PaymentMethod,
                Status = status,
                CreatedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error simulating payment for {Email}", request.CustomerEmail);
            throw;
        }
    }
}
