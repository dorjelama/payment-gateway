namespace payment_gateway_backend.Models;

public class PaymentResponseDto
{
    public string TransactionId { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
}
