#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CinemaSystem.API.Hubs;
using CinemaSystem.Application.Common.Interfaces;
using CinemaSystem.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace CinemaSystem.API.Services;

public class SignalRSeatNotifier : ISeatRealTimeNotifier
{
    private readonly IHubContext<CinemaHub> _hubContext;

    public SignalRSeatNotifier(IHubContext<CinemaHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifySeatsLockedAsync(Guid showtimeId, IEnumerable<Guid> seatIds, CancellationToken ct)
    {
        await _hubContext.Clients.Group(showtimeId.ToString()).SendAsync("SeatsLocked", seatIds, cancellationToken: ct);
    }

    public async Task NotifySeatsReleasedAsync(Guid showtimeId, IEnumerable<Guid> seatIds, CancellationToken ct)
    {
        await _hubContext.Clients.Group(showtimeId.ToString()).SendAsync("SeatsReleased", seatIds, cancellationToken: ct);
    }

    public async Task NotifySeatsConfirmedAsync(Guid showtimeId, IEnumerable<Guid> seatIds, CancellationToken ct)
    {
        await _hubContext.Clients.Group(showtimeId.ToString()).SendAsync("SeatsConfirmed", seatIds, cancellationToken: ct);
    }
}
