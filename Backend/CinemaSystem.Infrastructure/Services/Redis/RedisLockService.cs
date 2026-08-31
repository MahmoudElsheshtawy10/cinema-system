#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaSystem.Application.Interfaces;
using StackExchange.Redis;

namespace CinemaSystem.Infrastructure.Services.Redis;

public class RedisLockService : IRedisLockService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    public RedisLockService(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _db = redis.GetDatabase();
    }

    public async Task<bool> AcquireLockAsync(string key, string value, TimeSpan expiry)
    {
        return await _db.StringSetAsync(key, value, expiry, When.NotExists);
    }

    public async Task<bool> ReleaseLockAsync(string key, string value)
    {
        var script = @"
            if redis.call('get', KEYS[1]) == ARGV[1] then
                return redis.call('del', KEYS[1])
            else
                return 0
            end";

        var result = await _db.ScriptEvaluateAsync(script, new RedisKey[] { key }, new RedisValue[] { value });
        return (int)result == 1;
    }

    public async Task<bool> AcquireSeatsLockAsync(Guid showtimeId, IEnumerable<Guid> seatIds, string lockOwnerId, TimeSpan expiry)
    {
        var keys = seatIds.Select(id => (RedisKey)$"lock:showtime:{showtimeId}:seat:{id}").ToArray();
        if (keys.Length == 0) return true;

        var script = @"
            for i=1, #KEYS do
                if redis.call('exists', KEYS[i]) == 1 then
                    return 0
                end
            end
            for i=1, #KEYS do
                redis.call('set', KEYS[i], ARGV[1], 'PX', ARGV[2])
            end
            return 1";

        var result = await _db.ScriptEvaluateAsync(
            script, 
            keys, 
            new RedisValue[] { lockOwnerId, (long)expiry.TotalMilliseconds }
        );

        return (int)result == 1;
    }

    public async Task ReleaseSeatsLockAsync(Guid showtimeId, IEnumerable<Guid> seatIds, string lockOwnerId)
    {
        var keys = seatIds.Select(id => (RedisKey)$"lock:showtime:{showtimeId}:seat:{id}").ToArray();
        if (keys.Length == 0) return;

        var script = @"
            for i=1, #KEYS do
                if redis.call('get', KEYS[i]) == ARGV[1] then
                    redis.call('del', KEYS[i])
                end
            end
            return 1";

        await _db.ScriptEvaluateAsync(script, keys, new RedisValue[] { lockOwnerId });
    }

    public async Task<IReadOnlyDictionary<Guid, string>> GetLockedSeatsStatusAsync(Guid showtimeId, IEnumerable<Guid> seatIds)
    {
        var seatIdList = seatIds.ToList();
        var keys = seatIdList.Select(id => (RedisKey)$"lock:showtime:{showtimeId}:seat:{id}").ToArray();
        var values = await _db.StringGetAsync(keys);

        var result = new Dictionary<Guid, string>();
        for (int i = 0; i < seatIdList.Count; i++)
        {
            if (values[i].HasValue)
            {
                result[seatIdList[i]] = values[i]!;
            }
        }

        return result;
    }
}
