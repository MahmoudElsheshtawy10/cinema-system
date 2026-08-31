#nullable enable
using System;

namespace CinemaSystem.Application.DTOs;

public record PaymentRequest(
    Guid BookingId,
    decimal Amount,
    string Currency = "USD"
);
