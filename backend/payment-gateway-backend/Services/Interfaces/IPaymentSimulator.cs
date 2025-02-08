using payment_gateway_backend.Entities;
using payment_gateway_backend.Models.Payment;

namespace payment_gateway_backend.Services.Interfaces;

public interface IPaymentSimulator
{
    Task<PaymentTransaction> SimulatePaymentAsync(PaymentRequestDto request);
    Task<string> SimulateTransactionUpdateAsync(Guid transactionId);
}
