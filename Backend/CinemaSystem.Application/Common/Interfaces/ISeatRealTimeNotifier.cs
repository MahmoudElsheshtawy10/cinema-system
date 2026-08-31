#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CinemaSystem.Application.Common.Interfaces;

public interface ISeatRealTimeNotifier
{
    Task NotifySeatsLockedAsync(Guid showtimeId, IEnumerable<Guid> seatIds, CancellationToken ct);
    Task NotifySeatsReleasedAsync(Guid showtimeId, IEnumerable<Guid> seatIds, CancellationToken ct);
    Task NotifySeatsConfirmedAsync(Guid showtimeId, IEnumerable<Guid> seatIds, CancellationToken ct);
}
