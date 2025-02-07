namespace payment_gateway_backend.Models.PaymentTransaction
{
    public class CreatePaymentTransactionDto
    {
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentMethod { get; set; }
    }
}