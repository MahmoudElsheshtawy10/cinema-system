#nullable enable
using System;

namespace CinemaSystem.Application.DTOs;

public record TicketEmailDto(
    string MovieTitle,
    DateTime Showtime,
    string CinemaBranch,
    string HallName,
    string SeatDetails,
    string TicketCode,
    string QRCodePayload
);
