using LymmHolidayLets.Application.Interface.Service;
using Microsoft.Extensions.Caching.Memory;

namespace LymmHolidayLets.Infrastructure.Services;

public sealed class ApplicationMemoryCache(IMemoryCache cache) : IApplicationCache, IDisposable
{
    public bool TryGetValue<T>(string key, out T? value)
    {
        if (cache.TryGetValue(key, out T? cached))
        {
            value = cached;
            return true;
        }

        value = default;
        return false;
    }

    public void SetAbsolute<T>(string key, T value, TimeSpan ttl)
    {
        cache.Set(key, value, new MemoryCacheEntryOptions
        {
            Priority = CacheItemPriority.Normal,
            AbsoluteExpirationRelativeToNow = ttl
        });
    }

    public void SetAbsolute<T>(string key, T value, DateTimeOffset absoluteExpiration)
    {
        cache.Set(key, value, new MemoryCacheEntryOptions
        {
            Priority = CacheItemPriority.Normal,
            AbsoluteExpiration = absoluteExpiration
        });
    }

    public void SetSliding<T>(string key, T value, TimeSpan ttl)
    {
        cache.Set(key, value, new MemoryCacheEntryOptions
        {
            Priority = CacheItemPriority.NeverRemove,
            SlidingExpiration = ttl
        });
    }

    public void Remove(string key) => cache.Remove(key);

    public void Dispose()
    {
        if (cache is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
