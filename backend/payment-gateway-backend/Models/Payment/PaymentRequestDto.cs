namespace payment_gateway_backend.Models.Payment;

public class PaymentRequestDto
{
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public string PaymentMethod { get; set; }
    public string CardOrAccountNumber { get; set; }
    public int CVV { get; set; }
}
