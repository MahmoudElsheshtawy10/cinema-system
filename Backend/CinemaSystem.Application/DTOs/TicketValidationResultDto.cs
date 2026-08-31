#nullable enable
using System;

namespace CinemaSystem.Application.DTOs;

public record TicketValidationResultDto(
    bool IsValid,
    string Message,
    string? SeatRowLabel,
    int? SeatNumber,
    string? HallName
);
