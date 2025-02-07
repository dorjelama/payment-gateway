namespace payment_gateway_backend.Models.Payment
{
    public class PaymentStatusUpdatedEvent
    {
        public Guid TransactionId { get; set; }
        public string Status { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
