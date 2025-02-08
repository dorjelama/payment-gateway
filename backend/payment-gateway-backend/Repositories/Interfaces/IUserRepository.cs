using payment_gateway_backend.Entities;

namespace payment_gateway_backend.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User> GetUserByUsernameAsync(string username);
}
