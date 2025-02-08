using Microsoft.AspNetCore.Mvc;
using payment_gateway_backend.Models.Payment;

namespace payment_gateway_backend.Services.Interfaces
{
    public interface IPaymentProcessService
    {
        public Task<PaymentResponseDto> ProcessPaymentAsync(PaymentRequestDto request);
        public Task<PaymentResponseDto> RetryProcessPaymentAsync(PaymentRequestDto request, Guid transactionId);
    }
}