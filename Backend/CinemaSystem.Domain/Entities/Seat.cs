#nullable enable
using System;
using CinemaSystem.Domain.Common;
using CinemaSystem.Domain.Enums;

namespace CinemaSystem.Domain.Entities;

public class Seat : AuditableEntity<Guid>
{
    public Guid HallId { get; private set; }
    public string RowLabel { get; private set; } = string.Empty;
    public int SeatNumber { get; private set; }
    public SeatType SeatType { get; private set; }
    public int GridRow { get; private set; }
    public int GridCol { get; private set; }
    public bool IsActive { get; private set; }

    private Seat() { } // For EF Core

    public Seat(Guid hallId, string rowLabel, int seatNumber, SeatType seatType, int gridRow, int gridCol)
    {
        Id = Guid.NewGuid();
        HallId = hallId;
        RowLabel = rowLabel;
        SeatNumber = seatNumber;
        SeatType = seatType;
        GridRow = gridRow;
        GridCol = gridCol;
        IsActive = true;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
