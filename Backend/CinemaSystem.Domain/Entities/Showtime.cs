#nullable enable
using System;
using CinemaSystem.Domain.Common;

namespace CinemaSystem.Domain.Entities;

public class Showtime : AuditableEntity<Guid>
{
    public Guid MovieId { get; private set; }
    public Guid HallId { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public string Format { get; private set; } = string.Empty;
    public decimal BasePrice { get; private set; }
    public bool IsActive { get; private set; }

    private Showtime() { } // For EF Core

    public Showtime(Guid movieId, Guid hallId, DateTime startTime, DateTime endTime, string format, decimal basePrice)
    {
        Id = Guid.NewGuid();
        MovieId = movieId;
        HallId = hallId;
        StartTime = startTime;
        EndTime = endTime;
        Format = format;
        BasePrice = basePrice;
        IsActive = true;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
