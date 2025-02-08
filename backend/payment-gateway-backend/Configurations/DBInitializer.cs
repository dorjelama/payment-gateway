using Microsoft.EntityFrameworkCore;
using payment_gateway_backend.Data;
using payment_gateway_backend.Entities;

namespace payment_gateway_backend.Configurations;

public static class DbInitializer
{
    public static void Seed(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<PaymentGatewayDbContext>();

            context.Database.Migrate();

            if (!context.Users.Any(u => u.Username == "testuser"))
            {
                var adminUser = new User
                {
                    UserId = Guid.NewGuid(),
                    Username = "testuser",
                    Email = "testuser@example.com",
                    FullName = "Test User",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("testpassword"),
                    CreatedAt = DateTime.UtcNow
                };

                context.Users.Add(adminUser);
                context.SaveChanges();
            }
        }
    }
}
