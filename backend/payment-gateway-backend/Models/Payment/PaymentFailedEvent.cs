namespace payment_gateway_backend.Models.Payment
{
    public class PaymentFailedEvent
    {
        public Guid TransactionId { get; set; }
        public string ErrorMessage { get; set; }
    }
}
