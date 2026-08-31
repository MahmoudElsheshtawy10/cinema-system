#nullable enable
using System.Threading.Tasks;
using CinemaSystem.Application.Features.Staff.Commands.ValidateTicket;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CinemaSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StaffController : ControllerBase
{
    private readonly ISender _sender;

    public StaffController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("validate-ticket")]
    public async Task<IActionResult> ValidateTicket([FromBody] ValidateTicketCommand command)
    {
        var result = await _sender.Send(command);
        return Ok(result);
    }
}
