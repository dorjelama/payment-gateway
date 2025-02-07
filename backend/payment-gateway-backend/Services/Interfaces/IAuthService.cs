using payment_gateway_backend.Models;

namespace payment_gateway_backend.Services.Interfaces;
public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
}
