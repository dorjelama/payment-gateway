using payment_gateway_backend.Services.Interfaces;

namespace payment_gateway_backend.Services.Implementations;

public class EmailNotificationService : INotificationService
{
    private readonly ILogger<EmailNotificationService> _logger;

    public EmailNotificationService(ILogger<EmailNotificationService> logger)
    {
        _logger = logger;
    }

    public async Task SendNotificationAsync(string subject, string message)
    {
        // Simulate sending an email
        _logger.LogInformation("Sending email notification: Subject: {Subject}, Message: {Message}", subject, message);

        // Replace with actual email-sending logic
        await Task.Delay(100); // Simulate async operation
    }
}
