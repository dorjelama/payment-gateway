using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using payment_gateway_backend.Models.PaymentTransaction;
using payment_gateway_backend.Services.Interfaces;

namespace payment_gateway_backend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly IPaymentTransactionService _transactionService;

    public TransactionsController(IPaymentTransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTransactions([FromQuery] TransactionFilterDto filter)
    {
        var transactions = await _transactionService.GetTransactionsAsync(filter);
        return Ok(transactions);
    }
}
