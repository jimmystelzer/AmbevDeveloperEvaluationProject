using System.Text.Json;
using Ambev.DeveloperEvaluation.Domain.Services;
using Microsoft.Extensions.Caching.Distributed;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    public RedisCacheService(IDistributedCache cache) => _cache = cache;

    public async Task SetAsync(string key, object value, TimeSpan expiration)
    {
        var json = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        });
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await _cache.GetStringAsync(key);
        return json is null ? default : JsonSerializer.Deserialize<T>(json);
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }

    public async Task AddToListAsync(string key, string value)
    {
        var json = await _cache.GetStringAsync(key);
        var values = json is null ? new List<string>() : JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();

        if (!values.Contains(value))
        {
            values.Add(value);
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(values));
        }
    }

    public async Task RemoveListAsync(string key)
    {
        var json = await _cache.GetStringAsync(key);
        var values = json is null ? default : JsonSerializer.Deserialize<List<string>>(json);
        if (values is null) return;

        foreach (var value in values)
        {
            await _cache.RemoveAsync(value);
        }

        await _cache.RemoveAsync(key);
    }
}