namespace payment_gateway_backend.Services.Interfaces;

public interface INotificationService
{
    Task SendNotificationAsync(string subject, string message);
}
