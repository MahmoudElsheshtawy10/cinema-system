#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using CinemaSystem.Domain.Common;
using CinemaSystem.Domain.Enums;

namespace CinemaSystem.Domain.Entities;

public class Booking : AuditableEntity<Guid>
{
    public Guid UserId { get; private set; }
    public Guid ShowtimeId { get; private set; }
    public Guid? CouponId { get; private set; }
    public string BookingReference { get; private set; } = string.Empty;
    public decimal TotalAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal FinalAmount { get; private set; }
    public BookingStatus Status { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }

    private readonly List<BookingSeat> _bookingSeats = new();
    public IReadOnlyCollection<BookingSeat> BookingSeats => _bookingSeats.AsReadOnly();

    private readonly List<BookingFoodItem> _bookingFoodItems = new();
    public IReadOnlyCollection<BookingFoodItem> BookingFoodItems => _bookingFoodItems.AsReadOnly();

    private readonly List<Payment> _payments = new();
    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

    private Booking() { } // For EF Core

    public Booking(Guid userId, Guid showtimeId, string bookingReference, DateTime expiresAtUtc)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        ShowtimeId = showtimeId;
        BookingReference = bookingReference;
        Status = BookingStatus.Pending;
        ExpiresAtUtc = expiresAtUtc;
    }

    public void AddSeat(Seat seat, decimal price)
    {
        var bookingSeat = new BookingSeat(Id, seat.Id, price);
        _bookingSeats.Add(bookingSeat);
        CalculateTotals();
    }

    public void AddFoodItem(FoodItem item, int quantity)
    {
        var bookingFoodItem = new BookingFoodItem(Id, item.Id, quantity, item.Price);
        _bookingFoodItems.Add(bookingFoodItem);
        CalculateTotals();
    }

    public void ApplyDiscount(decimal discountAmount, Guid couponId)
    {
        CouponId = couponId;
        DiscountAmount = discountAmount;
        CalculateTotals();
    }

    public void ApplyDiscount(decimal discountAmount)
    {
        DiscountAmount = discountAmount;
        CalculateTotals();
    }

    public void Confirm()
    {
        Status = BookingStatus.Confirmed;
    }

    public void Cancel()
    {
        Status = BookingStatus.Cancelled;
    }

    public void Expire()
    {
        Status = BookingStatus.Expired;
    }

    private void CalculateTotals()
    {
        TotalAmount = _bookingSeats.Sum(s => s.Price) + _bookingFoodItems.Sum(f => f.TotalPrice);
        FinalAmount = TotalAmount - DiscountAmount;
        if (FinalAmount < 0) FinalAmount = 0;
    }
}
