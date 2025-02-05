using Microsoft.AspNetCore.Mvc;
using payment_gateway_backend.Helpers;
using payment_gateway_backend.Models;

namespace payment_gateway_backend.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly JwtHelper _jwtHelper;

    public AuthController(JwtHelper jwtHelper)
    {
        _jwtHelper = jwtHelper;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequestDto request)
    {
        if (request.Username == "admin" && request.Password == "password")
        {
            var token = _jwtHelper.GenerateToken(request.Username);
            return Ok(new { Token = token });
        }

        return Unauthorized(new { Message = "Invalid credentials" });
    }
}
