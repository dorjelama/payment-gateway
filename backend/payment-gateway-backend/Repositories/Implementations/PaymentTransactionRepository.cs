using Microsoft.EntityFrameworkCore;
using payment_gateway_backend.Data;
using payment_gateway_backend.Entities;
using payment_gateway_backend.Repositories.Interfaces;

namespace payment_gateway_backend.Repositories.Implementations
{
    public class PaymentTransactionRepository : IPaymentTransactionRepository
    {
        private readonly PaymentGatewayDbContext _context;

        public PaymentTransactionRepository(PaymentGatewayDbContext context)
        {
            _context = context;
        }

        public async Task AddTransactionAsync(PaymentTransaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<PaymentTransaction>> GetAllTransactionsAsync()
        {
            return await _context.Transactions.ToListAsync();
        }

        public async Task<PaymentTransaction> GetTransactionAsync(Guid transactionId)
        {
            return await _context.Transactions.FirstOrDefaultAsync(t => t.TransactionId == transactionId);
        }

        public async Task<bool> UpdateTransactionStatusAsync(Guid transactionId, string newStatus)
        {
            var oldTransaction = await _context.Transactions.FirstOrDefaultAsync(t => t.TransactionId == transactionId);
            if (oldTransaction == null)
            {
                return false;
            }

            PaymentTransaction newTransaction = new()
            {
                TransactionId = transactionId,
                Amount = oldTransaction.Amount,
                CreatedAt = DateTime.UtcNow,
                Currency = oldTransaction.Currency,
                PaymentMethod = oldTransaction.PaymentMethod,
                Status = newStatus,
                UserId = oldTransaction.UserId
            };

            await _context.Transactions.AddAsync(newTransaction);

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<PaymentTransaction>> GetTransactionsAsync(DateTime? startDate, DateTime? endDate, string? status)
        {
            var query = _context.Transactions.AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt <= endDate.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(t => t.Status == status);
            }

            return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }
    }
}
