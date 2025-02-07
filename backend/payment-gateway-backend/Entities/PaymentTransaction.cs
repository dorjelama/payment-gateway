using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace payment_gateway_backend.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class PaymentTransaction
{
    [Key]
    public Guid TransactionId { get; set; } = Guid.NewGuid(); // Primary Key

    [Required]
    public Guid UserId { get; set; } // Foreign Key

    [ForeignKey("UserId")]
    public User User { get; set; } // Navigation Property

    [Required]
    public decimal Amount { get; set; }

    [Required, MaxLength(3)]
    public string Currency { get; set; } = "USD";

    [Required, MaxLength(50)]
    public string PaymentMethod { get; set; } // Example: Card, PayPal, etc.

    [Required, MaxLength(50)]
    public string Status { get; set; } = "Pending"; // Example: Pending, Completed, Failed

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
