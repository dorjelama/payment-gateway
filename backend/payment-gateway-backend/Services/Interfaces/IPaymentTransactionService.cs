using payment_gateway_backend.Entities;
using payment_gateway_backend.Models.PaymentTransaction;

namespace payment_gateway_backend.Services.Interfaces
{
    public interface IPaymentTransactionService
    {
        Task<PaymentTransactionDto> CreateTransactionAsync(CreatePaymentTransactionDto request);
        Task<IEnumerable<PaymentTransactionDto>> GetTransactionsAsync(TransactionFilterDto filter);
        Task<PaymentTransactionDto> GetTransactionByIdAsync(Guid transactionId);
        Task<bool> UpdateTransactionStatusAsync(Guid transactionId, UpdateTransactionStatusDto updateRequest);
    }
}
