#nullable enable
using System;
using CinemaSystem.Domain.Common;

namespace CinemaSystem.Domain.Entities;

public class BookingFoodItem : BaseEntity<Guid>
{
    public Guid BookingId { get; private set; }
    public Guid FoodItemId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice { get; private set; }

    private BookingFoodItem() { } // For EF Core

    public BookingFoodItem(Guid bookingId, Guid foodItemId, int quantity, decimal unitPrice)
    {
        Id = Guid.NewGuid();
        BookingId = bookingId;
        FoodItemId = foodItemId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalPrice = quantity * unitPrice;
    }
}
