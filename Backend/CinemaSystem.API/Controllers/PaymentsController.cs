#nullable enable
using System.Threading.Tasks;
using CinemaSystem.Application.Features.Bookings.Commands.ConfirmPaymentWebhook;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CinemaSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly ISender _sender;

    public PaymentsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("paymob-webhook")]
    public async Task<IActionResult> PaymobWebhook([FromBody] ConfirmPaymentWebhookCommand command)
    {
        var result = await _sender.Send(command);
        return Ok(result);
    }
}
