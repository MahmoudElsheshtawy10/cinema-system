#nullable enable
using System;
using MediatR;
using CinemaSystem.Application.DTOs;

namespace CinemaSystem.Application.Features.Showtimes.Queries.GetShowtimeSeatLayout;

public record GetShowtimeSeatLayoutQuery(Guid ShowtimeId) : IRequest<SeatLayoutDto>;
