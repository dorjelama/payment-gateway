using Microsoft.AspNetCore.Mvc;
using payment_gateway_backend.Models.Auth;
using payment_gateway_backend.Services.Interfaces;

namespace payment_gateway_backend.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IEventLogService _eventLogService;

    public AuthController(IAuthService authService, IEventLogService eventLogService)
    {
        _authService = authService;
        _eventLogService = eventLogService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var response = await _authService.LoginAsync(request);

        if (string.IsNullOrEmpty(response.Token))
        {
            await _eventLogService.AddLogAsync("Failed Login Attempt", "Login failed for user: " + request.Username);
            return Unauthorized(response);
        }

        await _eventLogService.AddLogAsync("Successful Login Attempt", $"User {request.Username} logged in");
        return Ok(response);
    }
}