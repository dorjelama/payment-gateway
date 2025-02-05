using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using payment_gateway_backend.Models;
using Serilog;

namespace payment_gateway_backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ILogger<PaymentsController> _logger;
    public PaymentsController(IMapper mapper, ILogger<PaymentsController> logger)
    {
        _mapper = mapper;
        _logger = logger;
    }

    [Authorize]
    [HttpPost("ProcessPayment")]
    public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequestDto request)
    {
        if (!IsValidPaymentRequest(request, out string validationMessage))
        {
            _logger.LogWarning("Invalid payment request received: {ValidationMessage}", validationMessage);
            return BadRequest(new { message = validationMessage });
        }

        _logger.LogInformation("Processing payment for {CustomerEmail}, Amount: {Amount} {Currency}",
            request.CustomerEmail, request.Amount, request.Currency);

        var transaction = await SimulatePaymentGateway(request);

        _logger.LogInformation("Payment {Status} for {CustomerEmail}, Transaction ID: {TransactionId}",
            transaction.Status, transaction.CustomerEmail, transaction.TransactionId);

        return Ok(_mapper.Map<PaymentResponseDto>(transaction));
    }

    //need to make this api and call it from process payment
    private async Task<PaymentTransaction> SimulatePaymentGateway(PaymentRequestDto request)
    {
        await Task.Delay(3000);

        string[] statuses = { "Success", "Pending", "Failed" };
        Random random = new Random();
        string status = statuses[random.Next(statuses.Length)];

        return new PaymentTransaction
        {
            TransactionId = Guid.NewGuid(),
            Amount = request.Amount,
            Currency = request.Currency,
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            PaymentMethod = request.PaymentMethod,
            Status = status
        };
    }
    private bool IsValidPaymentRequest(PaymentRequestDto request, out string validationMessage)
    {
        if (request.Amount <= 0)
        {
            validationMessage = "Amount must be greater than zero.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(request.Currency))
        {
            validationMessage = "Currency is required.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(request.CustomerEmail))
        {
            validationMessage = "Customer email is required.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(request.PaymentMethod))
        {
            validationMessage = "Payment method is required.";
            return false;
        }

        validationMessage = string.Empty;
        return true;
    }
}
