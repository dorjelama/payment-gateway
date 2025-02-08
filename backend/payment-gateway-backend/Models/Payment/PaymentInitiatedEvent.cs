namespace payment_gateway_backend.Models.Payment
{
    public class PaymentInitiatedEvent
    {
        public Guid TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string CustomerEmail { get; set; }
    }
}