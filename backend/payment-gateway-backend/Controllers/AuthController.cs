using Microsoft.AspNetCore.Mvc;
using payment_gateway_backend.Models;
using payment_gateway_backend.Services.Interfaces;

namespace payment_gateway_backend.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var response = await _authService.LoginAsync(request);

        if (string.IsNullOrEmpty(response.Token))
            return Unauthorized(response);

        return Ok(response);
    }
}