#nullable enable
using System;

namespace CinemaSystem.Application.DTOs;

public record ReservationTokenResult(
    string ReservationToken,
    DateTime ExpiresAtUtc
);
