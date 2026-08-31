#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using CinemaSystem.Application.Common.Exceptions;
using CinemaSystem.Application.Common.Interfaces;
using CinemaSystem.Application.Interfaces;
using CinemaSystem.Application.DTOs;
using CinemaSystem.Domain.Entities;
using MediatR;

namespace CinemaSystem.Application.Features.Bookings.Commands.LockSeats;

public class LockSeatsCommandHandler : IRequestHandler<LockSeatsCommand, ReservationTokenResult>
{
    private readonly IRepository<BookingSeat, Guid> _bookingSeatRepository;
    private readonly IRedisLockService _redisLockService;
    private readonly ISeatRealTimeNotifier _notifier;

    public LockSeatsCommandHandler(
        IRepository<BookingSeat, Guid> bookingSeatRepository,
        IRedisLockService redisLockService,
        ISeatRealTimeNotifier notifier)
    {
        _bookingSeatRepository = bookingSeatRepository;
        _redisLockService = redisLockService;
        _notifier = notifier;
    }

    public async Task<ReservationTokenResult> Handle(LockSeatsCommand request, CancellationToken cancellationToken)
    {
        var existingBookingSeats = await _bookingSeatRepository.FindAsync(bs => request.SeatIds.Contains(bs.SeatId), cancellationToken);
        if (existingBookingSeats.Any())
        {
            throw new SeatConflictException("One or more selected seats are already booked.");
        }

        var expiry = TimeSpan.FromMinutes(10);
        var lockOwnerId = request.UserId.ToString();

        var lockAcquired = await _redisLockService.AcquireSeatsLockAsync(request.ShowtimeId, request.SeatIds, lockOwnerId, expiry);
        
        if (!lockAcquired)
        {
            throw new SeatConflictException("One or more selected seats are currently locked by another user.");
        }

        await _notifier.NotifySeatsLockedAsync(request.ShowtimeId, request.SeatIds, cancellationToken);

        return new ReservationTokenResult(lockOwnerId, DateTime.UtcNow.Add(expiry));
    }
}
