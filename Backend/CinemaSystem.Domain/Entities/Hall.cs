#nullable enable
using System;
using System.Collections.Generic;
using CinemaSystem.Domain.Common;

namespace CinemaSystem.Domain.Entities;

public class Hall : AuditableEntity<Guid>
{
    public Guid BranchId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string HallType { get; private set; } = string.Empty;
    public int TotalSeats { get; private set; }

    private readonly List<Seat> _seats = new();
    public IReadOnlyCollection<Seat> Seats => _seats.AsReadOnly();

    private readonly List<Showtime> _showtimes = new();
    public IReadOnlyCollection<Showtime> Showtimes => _showtimes.AsReadOnly();

    private Hall() { } // For EF Core

    public Hall(Guid branchId, string name, string hallType, int totalSeats)
    {
        Id = Guid.NewGuid();
        BranchId = branchId;
        Name = name;
        HallType = hallType;
        TotalSeats = totalSeats;
    }

    public void AddSeat(Seat seat)
    {
        _seats.Add(seat);
    }

    public void AddShowtime(Showtime showtime)
    {
        _showtimes.Add(showtime);
    }
}
