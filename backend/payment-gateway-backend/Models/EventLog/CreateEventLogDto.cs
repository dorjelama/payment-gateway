namespace payment_gateway_backend.Models.EventLog;

public class CreateEventLogDto
{
    public string EventType { get; set; }
    public string EventData { get; set; }
}
