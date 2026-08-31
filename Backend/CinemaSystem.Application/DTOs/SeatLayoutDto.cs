#nullable enable
using System;
using System.Collections.Generic;

namespace CinemaSystem.Application.DTOs;

public record SeatLayoutDto(
    int TotalRows,
    int TotalCols,
    List<SeatStatusDto> Seats
);

public record SeatStatusDto(
    Guid SeatId,
    string RowLabel,
    int SeatNumber,
    int GridRow,
    int GridCol,
    string SeatType,
    string Status
);
