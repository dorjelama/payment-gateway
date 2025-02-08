namespace payment_gateway_backend.Models.Payment
{
    public class PaymentFailureEvent
    {
        public Guid TransactionId { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime Timestamp { get; set; }
        public string CorrelationId { get; set; } // Optional: For tracing
    }
}
