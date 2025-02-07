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
            var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.TransactionId == transactionId);
            if (transaction == null)
            {
                return false;
            }

            transaction.Status = newStatus;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<PaymentTransaction>> GetTransactionsAsync(DateTime? startDate, DateTime? endDate, string? status, Guid? userId)
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

            if (userId.HasValue)
            {
                query = query.Where(t => t.UserId == userId.Value);
            }

            return await query.ToListAsync();
        }
    }
}
