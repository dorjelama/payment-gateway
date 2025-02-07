using System.ComponentModel.DataAnnotations;

namespace payment_gateway_backend.Entities;
public class User
{
    [Key]
    public Guid UserId { get; set; } = Guid.NewGuid();

    [Required, MaxLength(100)]
    public string Username { get; set; }

    [Required, MaxLength(255)]
    public string FullName { get; set; }

    [Required, MaxLength(255)]
    public string Email { get; set; }

    [Required, MaxLength(255)]
    public string PasswordHash { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<PaymentTransaction> Transactions { get; set; } = new List<PaymentTransaction>();
}
