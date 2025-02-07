namespace payment_gateway_backend.Entities
{
    public class EventLog
    {
        public Guid Id { get; set; }
        public string EventType { get; set; }
        public string Payload { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
