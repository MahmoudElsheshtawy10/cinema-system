#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CinemaSystem.Application.Interfaces;

public interface IRedisLockService
{
    Task<bool> AcquireLockAsync(string key, string value, TimeSpan expiry);
    Task<bool> ReleaseLockAsync(string key, string value);
    Task<bool> AcquireSeatsLockAsync(Guid showtimeId, IEnumerable<Guid> seatIds, string lockOwnerId, TimeSpan expiry);
    Task ReleaseSeatsLockAsync(Guid showtimeId, IEnumerable<Guid> seatIds, string lockOwnerId);
    Task<IReadOnlyDictionary<Guid, string>> GetLockedSeatsStatusAsync(Guid showtimeId, IEnumerable<Guid> seatIds);
}
