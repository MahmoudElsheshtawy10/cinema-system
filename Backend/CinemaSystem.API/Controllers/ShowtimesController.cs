#nullable enable
using System;
using System.Threading.Tasks;
using CinemaSystem.Application.Features.Showtimes.Queries.GetShowtimeSeatLayout;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CinemaSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShowtimesController : ControllerBase
{
    private readonly ISender _sender;

    public ShowtimesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{id}/seats")]
    public async Task<IActionResult> GetShowtimeSeatLayout(Guid id)
    {
        var result = await _sender.Send(new GetShowtimeSeatLayoutQuery(id));
        return Ok(result);
    }
}
