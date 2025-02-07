using Microsoft.IdentityModel.Tokens;
using payment_gateway_backend.Entities;
using payment_gateway_backend.Helpers;
using payment_gateway_backend.Models.Auth;
using payment_gateway_backend.Repositories.Interfaces;
using payment_gateway_backend.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace payment_gateway_backend.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _config;
    private readonly JwtHelper _jwtHelper;

    public AuthService(IUserRepository userRepository, IConfiguration config, JwtHelper jwtHelper)
    {
        _userRepository = userRepository;
        _config = config;
        _jwtHelper = jwtHelper;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetUserByUsernameAsync(request.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return new AuthResponseDto { Message = "Invalid credentials" };
        }

        var token = _jwtHelper.GenerateToken(user.Username);
        return new AuthResponseDto { Token = token, Message = "Login successful" };
    }
}