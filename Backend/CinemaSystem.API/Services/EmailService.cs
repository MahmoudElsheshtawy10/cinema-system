#nullable enable
using System.Threading;
using System.Threading.Tasks;
using CinemaSystem.Application.Common.Interfaces;
using CinemaSystem.Application.Interfaces;
using CinemaSystem.Application.DTOs;
using Microsoft.Extensions.Logging;

namespace CinemaSystem.API.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendBookingConfirmationEmailAsync(string toEmail, TicketEmailDto ticketDto, CancellationToken ct)
    {
        _logger.LogInformation(
            "Sending Booking Confirmation Email to {Email}. Movie: {Movie}, Tickets: {TicketCode}, Showtime: {Showtime}",
            toEmail, ticketDto.MovieTitle, ticketDto.TicketCode, ticketDto.Showtime);
        
        return Task.CompletedTask;
    }
}
