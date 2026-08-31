#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CinemaSystem.Application.Common.Exceptions;
using CinemaSystem.Application.Common.Interfaces;
using CinemaSystem.Application.Interfaces;
using CinemaSystem.Application.DTOs;
using CinemaSystem.Domain.Entities;
using MediatR;

namespace CinemaSystem.Application.Features.Bookings.Commands.InitiatePayment;

public class InitiatePaymentCommandHandler : IRequestHandler<InitiatePaymentCommand, PaymentInitiationResult>
{
    private readonly IRedisLockService _redisLockService;
    private readonly IRepository<Showtime, Guid> _showtimeRepository;
    private readonly IRepository<Seat, Guid> _seatRepository;
    private readonly IRepository<FoodItem, Guid> _foodItemRepository;
    private readonly IRepository<Coupon, Guid> _couponRepository;
    private readonly IRepository<Booking, Guid> _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentService _paymentService;

    public InitiatePaymentCommandHandler(
        IRedisLockService redisLockService,
        IRepository<Showtime, Guid> showtimeRepository,
        IRepository<Seat, Guid> seatRepository,
        IRepository<FoodItem, Guid> foodItemRepository,
        IRepository<Coupon, Guid> couponRepository,
        IRepository<Booking, Guid> bookingRepository,
        IUnitOfWork unitOfWork,
        IPaymentService paymentService)
    {
        _redisLockService = redisLockService;
        _showtimeRepository = showtimeRepository;
        _seatRepository = seatRepository;
        _foodItemRepository = foodItemRepository;
        _couponRepository = couponRepository;
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
        _paymentService = paymentService;
    }

    public async Task<PaymentInitiationResult> Handle(InitiatePaymentCommand request, CancellationToken cancellationToken)
    {
        var lockedSeats = await _redisLockService.GetLockedSeatsStatusAsync(request.ShowtimeId, request.SeatIds);
        if (lockedSeats.Count != request.SeatIds.Count || lockedSeats.Values.Any(v => v != request.UserId.ToString()))
        {
            throw new ValidationException(new[] { new FluentValidation.Results.ValidationFailure("Seats", "Lock expired or owned by another user.") });
        }

        var showtime = await _showtimeRepository.GetByIdAsync(request.ShowtimeId, cancellationToken);
        if (showtime == null) throw new NotFoundException(nameof(Showtime), request.ShowtimeId);

        var seats = await _seatRepository.FindAsync(s => request.SeatIds.Contains(s.Id), cancellationToken);
        
        var bookingRef = $"BKG-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
        var booking = new Booking(request.UserId, request.ShowtimeId, bookingRef, DateTime.UtcNow.AddMinutes(10));

        foreach (var seat in seats)
        {
            booking.AddSeat(seat, showtime.BasePrice);
        }

        if (request.FoodItems != null && request.FoodItems.Any())
        {
            var foodItemIds = request.FoodItems.Select(f => f.FoodItemId).ToList();
            var foodItems = await _foodItemRepository.FindAsync(f => foodItemIds.Contains(f.Id), cancellationToken);
            foreach (var item in request.FoodItems)
            {
                var food = foodItems.FirstOrDefault(f => f.Id == item.FoodItemId);
                if (food != null)
                {
                    booking.AddFoodItem(food, item.Quantity);
                }
            }
        }

        if (!string.IsNullOrEmpty(request.CouponCode))
        {
            var coupon = await _couponRepository.FirstOrDefaultAsync(c => c.Code == request.CouponCode, cancellationToken);
            if (coupon != null && coupon.CanBeUsed())
            {
                var discount = coupon.DiscountType == Domain.Enums.DiscountType.FixedAmount 
                    ? coupon.DiscountValue 
                    : (booking.TotalAmount * (coupon.DiscountValue / 100m));
                booking.ApplyDiscount(discount, coupon.Id);
            }
        }

        await _bookingRepository.AddAsync(booking, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var paymentRequest = new PaymentRequest(booking.Id, booking.FinalAmount);
        var paymentResult = await _paymentService.InitiatePaymentAsync(paymentRequest, cancellationToken);

        return paymentResult;
    }
}
