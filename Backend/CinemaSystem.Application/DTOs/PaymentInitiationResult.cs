#nullable enable
namespace CinemaSystem.Application.DTOs;

public record PaymentInitiationResult(
    string TransactionReference,
    string CheckoutUrl
);
