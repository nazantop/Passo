using Microsoft.Extensions.Caching.Distributed;
using PassoCourseApp.Infrastructure.Caching;
using System.Text.Json;

namespace PassoCourseApp.Infrastructure.Extensions;

public static class CacheExtensions
{
    private static readonly TimeSpan OpTimeout = TimeSpan.FromMilliseconds(50);
    private static readonly TimeSpan Cooldown = TimeSpan.FromMinutes(5);

    public static async Task<T?> GetOrSetAsync<T>(
        this IDistributedCache cache,
        ICacheGate gate,
        string key,
        Func<Task<T>> factory,
        TimeSpan ttl)
    {
        if (!gate.IsOpen) return await factory();

        try
        {
            using var cts = new CancellationTokenSource(OpTimeout);
            var bytes = await cache.GetAsync(key, cts.Token);
            if (bytes != null) return JsonSerializer.Deserialize<T>(bytes);
        }
        catch { gate.Trip(Cooldown); return await factory(); }

        var data = await factory();

        try
        {
            if (data != null)
            {
                using var cts = new CancellationTokenSource(OpTimeout);
                var payload = JsonSerializer.SerializeToUtf8Bytes(data);
                await cache.SetAsync(key, payload,
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl },
                    cts.Token);
            }
        }
        catch { gate.Trip(Cooldown); }

        return data;
    }

    public static async Task RemoveMany(this IDistributedCache cache, ICacheGate gate, params string[] keys)
    {
        if (!gate.IsOpen) return;
        foreach (var k in keys)
        {
            try
            {
                using var cts = new CancellationTokenSource(OpTimeout);
                await cache.RemoveAsync(k, cts.Token);
            }
            catch { gate.Trip(Cooldown); return; }
        }
    }
}