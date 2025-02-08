using payment_gateway_backend.Entities;

namespace payment_gateway_backend.Repositories.Interfaces;

public interface IPaymentTransactionRepository
{
    public Task AddTransactionAsync(PaymentTransaction transaction);

    public Task<IEnumerable<PaymentTransaction>> GetAllTransactionsAsync();

    public Task<PaymentTransaction> GetTransactionAsync(Guid transactionId);

    public Task<bool> UpdateTransactionStatusAsync(Guid transactionId, string newStatus);
    public Task<IEnumerable<PaymentTransaction>> GetTransactionsAsync(DateTime? startDate, DateTime? endDate, string? status);
}
