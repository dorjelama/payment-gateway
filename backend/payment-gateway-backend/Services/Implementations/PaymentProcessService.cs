using Microsoft.AspNetCore.Mvc;
using payment_gateway_backend.Configurations;
using payment_gateway_backend.Entities;
using payment_gateway_backend.Helpers;
using payment_gateway_backend.Models;
using payment_gateway_backend.Models.Payment;
using payment_gateway_backend.Repositories.Interfaces;
using payment_gateway_backend.Services.Interfaces;

namespace payment_gateway_backend.Services.Implementations;

public class PaymentProcessService : IPaymentProcessService
{
    private readonly IPaymentTransactionRepository _transactionRepository;
    private readonly Publisher _publisher;
    private readonly ILogger<PaymentProcessService> _logger;
    private readonly IEventLogService _eventLogService;
    private readonly JwtHelper _jwtHelper;
    public PaymentProcessService(IPaymentTransactionRepository transactionRepository,
                                 Publisher publisher,
                                 ILogger<PaymentProcessService> logger,
                                 JwtHelper jwtHelper,
                                 IEventLogService eventLogService)
    {
        _transactionRepository = transactionRepository;
        _publisher = publisher;
        _logger = logger;
        _jwtHelper = jwtHelper;
        _eventLogService = eventLogService;
    }

    public async Task<PaymentResponseDto> ProcessPaymentAsync(PaymentRequestDto request)
    {

        if (!IsValidPaymentRequest(request, out string validationMessage))
        {
            _logger.LogWarning($"Invalid payment request received: {validationMessage}");
            await _eventLogService.AddLogAsync("Invalid Payment Request", validationMessage);
            throw new ArgumentException(validationMessage);
        }

        _logger.LogInformation($"Processing payment for {request.CustomerEmail}, Amount: {request.Amount} {request.Currency}");

        var transactionId = Guid.NewGuid();
        var transaction = new PaymentTransaction
        {
            Id = Guid.NewGuid(),
            TransactionId = transactionId,
            Amount = request.Amount,
            Currency = request.Currency,
            Status = "Initiated",
            CreatedAt = DateTime.UtcNow,
            PaymentMethod = request.PaymentMethod,
            UserId = _jwtHelper.GetUserIdFromToken()
        };

        try
        {
            await _transactionRepository.AddTransactionAsync(transaction);

            await _publisher.PublishEventAsync(new PaymentInitiatedEvent
            {
                TransactionId = transactionId,
                Amount = request.Amount,
                Currency = request.Currency,
                CustomerEmail = request.CustomerEmail
            }, "payment.initiated");

            _logger.LogInformation($"Payment initiated for {request.CustomerEmail}, Transaction ID: {transactionId}");
            await _eventLogService.AddLogAsync("Payment Initiated", $"Payment initiated for {request.CustomerEmail}, Transaction ID: {transactionId}");

            return new PaymentResponseDto
            {
                TransactionId = transactionId,
                Message = "Payment processing started."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment");
            throw;
        }
    }
    public async Task<PaymentResponseDto> RetryProcessPaymentAsync(PaymentRequestDto request, Guid transactionId)
    {

        _logger.LogInformation($"Reprocessing payment for transaction {transactionId}");

        var transaction = new PaymentTransaction
        {
            Id = Guid.NewGuid(),
            TransactionId = transactionId,
            Amount = request.Amount,
            Currency = request.Currency,
            Status = "Initiated",
            CreatedAt = DateTime.UtcNow,
            PaymentMethod = request.PaymentMethod,
            UserId = _jwtHelper.GetUserIdFromToken()
        };

        try
        {
            await _transactionRepository.AddTransactionAsync(transaction);

            await _publisher.PublishEventAsync(new PaymentInitiatedEvent
            {
                TransactionId = transactionId,
                Amount = request.Amount,
                Currency = request.Currency,
                CustomerEmail = request.CustomerEmail
            }, "payment.retrying");

            _logger.LogInformation($"Retrying payment process for Transaction ID: {transactionId}");
            await _eventLogService.AddLogAsync("Payment Initiated", $"Retrying payment process for Transaction ID: {transactionId}");

            return new PaymentResponseDto
            {
                TransactionId = transactionId,
                Message = "Retrying Payment processing."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment");
            throw;
        }
    }
    private bool IsValidPaymentRequest(PaymentRequestDto request, out string validationMessage)
    {
        if (request.Amount <= 0)
        {
            validationMessage = "Amount must be greater than zero.";
            return false;
        }
        if (string.IsNullOrEmpty(request.Currency) || request.Currency.Length != 3)
        {
            validationMessage = "Invalid currency format.";
            return false;
        }
        if (string.IsNullOrEmpty(request.CustomerEmail) || !request.CustomerEmail.Contains("@"))
        {
            validationMessage = "Invalid customer email.";
            return false;
        }
        validationMessage = string.Empty;
        return true;
    }
}
