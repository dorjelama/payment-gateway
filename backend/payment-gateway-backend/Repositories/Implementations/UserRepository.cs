using Microsoft.EntityFrameworkCore;
using payment_gateway_backend.Data;
using payment_gateway_backend.Entities;
using payment_gateway_backend.Repositories.Interfaces;

namespace payment_gateway_backend.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly PaymentGatewayDbContext _context;

    public UserRepository(PaymentGatewayDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetUserByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }
}
