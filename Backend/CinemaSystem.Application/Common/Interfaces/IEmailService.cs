#nullable enable
using System.Threading;
using System.Threading.Tasks;
using CinemaSystem.Application.DTOs;

namespace CinemaSystem.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendBookingConfirmationEmailAsync(string toEmail, TicketEmailDto ticketDto, CancellationToken ct);
}
