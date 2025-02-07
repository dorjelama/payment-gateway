namespace payment_gateway_backend.Models.PaymentTransaction
{
    public class TransactionFilterDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public Guid? UserId { get; set; }
    }
}
