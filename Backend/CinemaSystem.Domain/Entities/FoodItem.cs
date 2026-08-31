#nullable enable
using System;
using CinemaSystem.Domain.Common;

namespace CinemaSystem.Domain.Entities;

public class FoodItem : AuditableEntity<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public string? ImageUrl { get; private set; }
    public bool IsAvailable { get; private set; }

    private FoodItem() { } // For EF Core

    public FoodItem(string name, string category, decimal price)
    {
        Id = Guid.NewGuid();
        Name = name;
        Category = category;
        Price = price;
        IsAvailable = true;
    }

    public void SetImageUrl(string imageUrl) => ImageUrl = imageUrl;
    public void SetAvailability(bool isAvailable) => IsAvailable = isAvailable;
}
