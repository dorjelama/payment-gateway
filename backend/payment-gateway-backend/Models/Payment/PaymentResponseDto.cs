namespace payment_gateway_backend.Models.Payment;

public class PaymentResponseDto
{
    public Guid TransactionId { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
}
