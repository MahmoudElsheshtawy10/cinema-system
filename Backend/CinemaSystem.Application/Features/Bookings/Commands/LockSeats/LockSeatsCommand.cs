#nullable enable
using System;
using System.Collections.Generic;
using CinemaSystem.Application.DTOs;
using MediatR;

namespace CinemaSystem.Application.Features.Bookings.Commands.LockSeats;

public record LockSeatsCommand(
    Guid ShowtimeId,
    List<Guid> SeatIds,
    Guid UserId
) : IRequest<ReservationTokenResult>;
