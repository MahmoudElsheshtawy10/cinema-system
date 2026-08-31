#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using CinemaSystem.Application.Common.Interfaces;
using CinemaSystem.Application.Interfaces;
using CinemaSystem.Application.DTOs;
using CinemaSystem.Domain.Entities;
using MediatR;

namespace CinemaSystem.Application.Features.Staff.Commands.ValidateTicket;

public class ValidateTicketCommandHandler : IRequestHandler<ValidateTicketCommand, TicketValidationResultDto>
{
    private readonly IRepository<Ticket, Guid> _ticketRepository;
    private readonly IRepository<BookingSeat, Guid> _bookingSeatRepository;
    private readonly IRepository<Seat, Guid> _seatRepository;
    private readonly IRepository<Booking, Guid> _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ValidateTicketCommandHandler(
        IRepository<Ticket, Guid> ticketRepository,
        IRepository<BookingSeat, Guid> bookingSeatRepository,
        IRepository<Seat, Guid> seatRepository,
        IRepository<Booking, Guid> bookingRepository,
        IUnitOfWork unitOfWork)
    {
        _ticketRepository = ticketRepository;
        _bookingSeatRepository = bookingSeatRepository;
        _seatRepository = seatRepository;
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TicketValidationResultDto> Handle(ValidateTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.FirstOrDefaultAsync(t => t.TicketCode == request.TicketCode, cancellationToken);
        
        if (ticket == null)
            return new TicketValidationResultDto(false, "Ticket not found.", null, null, null);

        if (ticket.IsCheckedIn)
            return new TicketValidationResultDto(false, "Ticket has already been checked in.", null, null, null);

        var bookingSeat = await _bookingSeatRepository.GetByIdAsync(ticket.BookingSeatId, cancellationToken);
        if (bookingSeat == null)
            return new TicketValidationResultDto(false, "Associated booking seat not found.", null, null, null);

        var booking = await _bookingRepository.GetByIdAsync(bookingSeat.BookingId, cancellationToken);
        if (booking == null || booking.ShowtimeId != request.ShowtimeId)
            return new TicketValidationResultDto(false, "Ticket is not for this showtime.", null, null, null);

        var seat = await _seatRepository.GetByIdAsync(bookingSeat.SeatId, cancellationToken);
        
        ticket.ValidateAndCheckIn(request.StaffUserId);
        _ticketRepository.Update(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new TicketValidationResultDto(true, "Ticket checked in successfully.", seat?.RowLabel, seat?.SeatNumber, null);
    }
}
