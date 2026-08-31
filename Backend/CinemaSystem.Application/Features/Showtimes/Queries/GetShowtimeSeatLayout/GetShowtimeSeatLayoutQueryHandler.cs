#nullable enable
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CinemaSystem.Application.Common.Exceptions;
using CinemaSystem.Application.Common.Interfaces;
using CinemaSystem.Application.Interfaces;
using CinemaSystem.Application.DTOs;
using CinemaSystem.Domain.Entities;
using MediatR;

namespace CinemaSystem.Application.Features.Showtimes.Queries.GetShowtimeSeatLayout;

public class GetShowtimeSeatLayoutQueryHandler : IRequestHandler<GetShowtimeSeatLayoutQuery, SeatLayoutDto>
{
    private readonly IRepository<Showtime, Guid> _showtimeRepository;
    private readonly IRepository<Seat, Guid> _seatRepository;
    private readonly IRepository<BookingSeat, Guid> _bookingSeatRepository;
    private readonly IRepository<Booking, Guid> _bookingRepository;
    private readonly IRedisLockService _redisLockService;

    public GetShowtimeSeatLayoutQueryHandler(
        IRepository<Showtime, Guid> showtimeRepository,
        IRepository<Seat, Guid> seatRepository,
        IRepository<BookingSeat, Guid> bookingSeatRepository,
        IRepository<Booking, Guid> bookingRepository,
        IRedisLockService redisLockService)
    {
        _showtimeRepository = showtimeRepository;
        _seatRepository = seatRepository;
        _bookingSeatRepository = bookingSeatRepository;
        _bookingRepository = bookingRepository;
        _redisLockService = redisLockService;
    }

    public async Task<SeatLayoutDto> Handle(GetShowtimeSeatLayoutQuery request, CancellationToken cancellationToken)
    {
        var showtime = await _showtimeRepository.GetByIdAsync(request.ShowtimeId, cancellationToken);
        if (showtime == null) throw new NotFoundException(nameof(Showtime), request.ShowtimeId);

        var seats = await _seatRepository.FindAsync(s => s.HallId == showtime.HallId, cancellationToken);
        
        var bookings = await _bookingRepository.FindAsync(b => b.ShowtimeId == request.ShowtimeId && b.Status != Domain.Enums.BookingStatus.Cancelled && b.Status != Domain.Enums.BookingStatus.Expired, cancellationToken);
        var bookingIds = bookings.Select(b => b.Id).ToList();
        var bookingSeats = await _bookingSeatRepository.FindAsync(bs => bookingIds.Contains(bs.BookingId), cancellationToken);
        var bookedSeatIds = bookingSeats.Select(bs => bs.SeatId).ToHashSet();

        var seatIds = seats.Select(s => s.Id).ToList();
        var lockedSeats = await _redisLockService.GetLockedSeatsStatusAsync(request.ShowtimeId, seatIds);

        var seatStatusDtos = new System.Collections.Generic.List<SeatStatusDto>();

        int maxRow = 0;
        int maxCol = 0;

        foreach (var seat in seats)
        {
            if (seat.GridRow > maxRow) maxRow = seat.GridRow;
            if (seat.GridCol > maxCol) maxCol = seat.GridCol;

            string status = "Available";
            if (bookedSeatIds.Contains(seat.Id))
            {
                status = "Booked";
            }
            else if (lockedSeats.ContainsKey(seat.Id))
            {
                status = "Locked";
            }

            seatStatusDtos.Add(new SeatStatusDto(
                seat.Id,
                seat.RowLabel,
                seat.SeatNumber,
                seat.GridRow,
                seat.GridCol,
                seat.SeatType.ToString(),
                status
            ));
        }

        return new SeatLayoutDto(maxRow + 1, maxCol + 1, seatStatusDtos);
    }
}
