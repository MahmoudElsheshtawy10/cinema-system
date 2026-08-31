#nullable enable
using System;

namespace CinemaSystem.Application.DTOs;

public record FoodOrderItemDto(
    Guid FoodItemId,
    int Quantity
);
