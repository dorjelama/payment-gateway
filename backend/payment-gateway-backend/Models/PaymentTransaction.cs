namespace payment_gateway_backend.Models;

public class PaymentTransaction
{
    public Guid TransactionId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public string PaymentMethod { get; set; }
    public string Status { get; set; }
}
