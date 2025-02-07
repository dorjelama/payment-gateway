using AutoMapper;
using payment_gateway_backend.Entities;
using payment_gateway_backend.Models.PaymentTransaction;
using payment_gateway_backend.Repositories.Interfaces;
using payment_gateway_backend.Services.Interfaces;

namespace payment_gateway_backend.Services.Implementations;

public class PaymentTransactionService : IPaymentTransactionService
{
    private readonly IPaymentTransactionRepository _transactionRepository;
    private readonly IMapper _mapper;

    public PaymentTransactionService(IPaymentTransactionRepository transactionRepository, IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    public async Task<PaymentTransactionDto> CreateTransactionAsync(CreatePaymentTransactionDto request)
    {
        var transaction = new PaymentTransaction
        {
            UserId = request.UserId,
            Amount = request.Amount,
            Currency = request.Currency,
            PaymentMethod = request.PaymentMethod,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        await _transactionRepository.AddTransactionAsync(transaction);
        return _mapper.Map<PaymentTransactionDto>(transaction);
    }

    public async Task<IEnumerable<PaymentTransactionDto>> GetTransactionsAsync(TransactionFilterDto filter)
    {
        var transactions = await _transactionRepository.GetTransactionsAsync(
            filter.StartDate,
            filter.EndDate,
            filter.Status,
            filter.UserId);

        return _mapper.Map<IEnumerable<PaymentTransactionDto>>(transactions);
    }

    public async Task<PaymentTransactionDto> GetTransactionByIdAsync(Guid transactionId)
    {
        var transaction = await _transactionRepository.GetTransactionAsync(transactionId);
        return transaction != null ? _mapper.Map<PaymentTransactionDto>(transaction) : null;
    }

    public async Task<bool> UpdateTransactionStatusAsync(Guid transactionId, UpdateTransactionStatusDto updateRequest)
    {
        return await _transactionRepository.UpdateTransactionStatusAsync(transactionId, updateRequest.Status);
    }
}
