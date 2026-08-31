#nullable enable
using System;
using CinemaSystem.Domain.Common;

namespace CinemaSystem.Domain.Entities;

public class BookingSeat : BaseEntity<Guid>
{
    public Guid BookingId { get; private set; }
    public Guid SeatId { get; private set; }
    public decimal Price { get; private set; }

    private BookingSeat() { } // For EF Core

    public BookingSeat(Guid bookingId, Guid seatId, decimal price)
    {
        Id = Guid.NewGuid();
        BookingId = bookingId;
        SeatId = seatId;
        Price = price;
    }
}
