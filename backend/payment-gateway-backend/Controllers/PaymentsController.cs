using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using payment_gateway_backend.Configurations;
using payment_gateway_backend.Entities;
using payment_gateway_backend.Models.Payment;
using payment_gateway_backend.Repositories;
using payment_gateway_backend.Services.Implementations;
using payment_gateway_backend.Services.Interfaces;

namespace payment_gateway_backend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly ILogger<PaymentsController> _logger;
    private readonly IMapper _mapper;
    private readonly Publisher _publisher;
    private readonly IPaymentProcessService _paymentProcessService;
    public PaymentsController(
        ILogger<PaymentsController> logger,
        IMapper mapper,
        Publisher publisher,
        IPaymentProcessService paymentProcessService)
    {
        _logger = logger;
        _mapper = mapper;
        _publisher = publisher;
        _paymentProcessService = paymentProcessService;
    }

    [HttpPost("Process")]
    public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequestDto request)
    {
        try
        {
            var response = await _paymentProcessService.ProcessPaymentAsync(request);
            return Accepted(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An internal server error occurred. Please try again later." });
        }
    }

    [HttpPost("SimulatePaymentGateway")]
    public async Task<IActionResult> SimulatePaymentGateway([FromBody] PaymentRequestDto request, [FromServices] IPaymentSimulator simulator)
    {
        try
        {
            var transaction = await simulator.SimulatePaymentAsync(request);
            return Ok(transaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in simulation endpoint for {Email}", request.CustomerEmail);
            return StatusCode(500, "Simulation failed");
        }
    }
}
