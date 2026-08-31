#nullable enable
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CinemaSystem.Application.Common.Exceptions;
using CinemaSystem.Application.Common.Interfaces;
using CinemaSystem.Application.Interfaces;
using CinemaSystem.Domain.Entities;
using MediatR;
using CinemaSystem.Application.DTOs;

namespace CinemaSystem.Application.Features.Bookings.Commands.ConfirmPaymentWebhook;

public class ConfirmPaymentWebhookCommandHandler : IRequestHandler<ConfirmPaymentWebhookCommand, bool>
{
    private readonly IPaymentService _paymentService;
    private readonly IRepository<Booking, Guid> _bookingRepository;
    private readonly IRepository<Ticket, Guid> _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IQRGeneratorService _qrGeneratorService;
    private readonly ISeatRealTimeNotifier _notifier;
    private readonly IRedisLockService _redisLockService;
    private readonly IEmailService _emailService;
    private readonly IRepository<User, Guid> _userRepository;
    private readonly IRepository<Showtime, Guid> _showtimeRepository;
    private readonly IRepository<Movie, Guid> _movieRepository;
    private readonly IRepository<Hall, Guid> _hallRepository;
    private readonly IRepository<CinemaBranch, Guid> _branchRepository;
    private readonly IRepository<Seat, Guid> _seatRepository;

    public ConfirmPaymentWebhookCommandHandler(
        IPaymentService paymentService,
        IRepository<Booking, Guid> bookingRepository,
        IRepository<Ticket, Guid> ticketRepository,
        IUnitOfWork unitOfWork,
        IQRGeneratorService qrGeneratorService,
        ISeatRealTimeNotifier notifier,
        IRedisLockService redisLockService,
        IEmailService emailService,
        IRepository<User, Guid> userRepository,
        IRepository<Showtime, Guid> showtimeRepository,
        IRepository<Movie, Guid> movieRepository,
        IRepository<Hall, Guid> hallRepository,
        IRepository<CinemaBranch, Guid> branchRepository,
        IRepository<Seat, Guid> seatRepository)
    {
        _paymentService = paymentService;
        _bookingRepository = bookingRepository;
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
        _qrGeneratorService = qrGeneratorService;
        _notifier = notifier;
        _redisLockService = redisLockService;
        _emailService = emailService;
        _userRepository = userRepository;
        _showtimeRepository = showtimeRepository;
        _movieRepository = movieRepository;
        _hallRepository = hallRepository;
        _branchRepository = branchRepository;
        _seatRepository = seatRepository;
    }

    public async Task<bool> Handle(ConfirmPaymentWebhookCommand request, CancellationToken cancellationToken)
    {
        if (!_paymentService.VerifyWebhookSignature(request.RawPayload, request.Signature))
        {
            throw new ValidationException(new[] { new FluentValidation.Results.ValidationFailure("Signature", "Invalid webhook signature") });
        }

        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking == null) throw new NotFoundException(nameof(Booking), request.BookingId);

        var payment = booking.Payments.FirstOrDefault();
        if (payment != null)
        {
            payment.MarkAsCompleted();
        }
        
        booking.Confirm();

        var seatIds = booking.BookingSeats.Select(bs => bs.SeatId).ToList();
        
        foreach (var bookingSeat in booking.BookingSeats)
        {
            var ticketCode = $"TCK-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
            var qrPayload = $"{booking.Id}-{bookingSeat.Id}-{ticketCode}";
            var qrCode = await _qrGeneratorService.GenerateQRCodeAsync(qrPayload, cancellationToken);
            
            var ticket = new Ticket(bookingSeat.Id, ticketCode, qrCode);
            await _ticketRepository.AddAsync(ticket, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _redisLockService.ReleaseSeatsLockAsync(booking.ShowtimeId, seatIds, booking.UserId.ToString());
        await _notifier.NotifySeatsConfirmedAsync(booking.ShowtimeId, seatIds, cancellationToken);

        var user = await _userRepository.GetByIdAsync(booking.UserId, cancellationToken);
        var showtime = await _showtimeRepository.GetByIdAsync(booking.ShowtimeId, cancellationToken);
        if (user != null && showtime != null)
        {
            var movie = await _movieRepository.GetByIdAsync(showtime.MovieId, cancellationToken);
            var hall = await _hallRepository.GetByIdAsync(showtime.HallId, cancellationToken);
            var branch = hall != null ? await _branchRepository.GetByIdAsync(hall.BranchId, cancellationToken) : null;
            
            var ticketDto = new TicketEmailDto(
                movie?.Title ?? "Unknown Movie",
                showtime.StartTime,
                branch?.Name ?? "Unknown Branch",
                hall?.Name ?? "Unknown Hall",
                $"Seats: {seatIds.Count}",
                booking.BookingReference,
                "QR-Data"
            );
            await _emailService.SendBookingConfirmationEmailAsync(user.Email, ticketDto, cancellationToken);
        }

        return true;
    }
}
