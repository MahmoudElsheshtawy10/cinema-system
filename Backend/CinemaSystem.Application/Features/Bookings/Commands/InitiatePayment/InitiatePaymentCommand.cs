#nullable enable
using System;
using System.Collections.Generic;
using CinemaSystem.Application.DTOs;
using MediatR;

namespace CinemaSystem.Application.Features.Bookings.Commands.InitiatePayment;

public record InitiatePaymentCommand(
    Guid ShowtimeId,
    List<Guid> SeatIds,
    List<FoodOrderItemDto>? FoodItems,
    string? CouponCode,
    Guid UserId
) : IRequest<PaymentInitiationResult>;
