namespace LymmHolidayLets.Application.Interface.Service;

public interface IApplicationCache
{
    bool TryGetValue<T>(string key, out T? value);
    void SetAbsolute<T>(string key, T value, TimeSpan ttl);
    void SetAbsolute<T>(string key, T value, DateTimeOffset absoluteExpiration);
    void SetSliding<T>(string key, T value, TimeSpan ttl);
    void Remove(string key);
}
