#nullable enable
using System.Threading.Tasks;
using CinemaSystem.Application.Features.Bookings.Commands.InitiatePayment;
using CinemaSystem.Application.Features.Bookings.Commands.LockSeats;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CinemaSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly ISender _sender;

    public BookingsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("lock")]
    public async Task<IActionResult> LockSeats([FromBody] LockSeatsCommand command)
    {
        var result = await _sender.Send(command);
        return Ok(result);
    }

    [HttpPost("initiate-payment")]
    public async Task<IActionResult> InitiatePayment([FromBody] InitiatePaymentCommand command)
    {
        var result = await _sender.Send(command);
        return Ok(result);
    }
}
