namespace payment_gateway_backend.Models;

public record PaymentRequestDto(
    decimal Amount,
    string Currency,
    string CustomerName,
    string CustomerEmail,
    string PaymentMethod,
    string CardOrAccountNumber
);
