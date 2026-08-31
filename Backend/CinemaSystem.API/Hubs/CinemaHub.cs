#nullable enable
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace CinemaSystem.API.Hubs;

public class CinemaHub : Hub
{
    public async Task JoinShowtimeGroup(Guid showtimeId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, showtimeId.ToString());
    }

    public async Task LeaveShowtimeGroup(Guid showtimeId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, showtimeId.ToString());
    }
}
