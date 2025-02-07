using payment_gateway_backend.Models.Auth;

namespace payment_gateway_backend.Services.Interfaces;
public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
}
