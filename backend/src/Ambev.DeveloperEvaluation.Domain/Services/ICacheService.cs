namespace Ambev.DeveloperEvaluation.Domain.Services;

public interface ICacheService
{
    Task SetAsync(string key, object value, TimeSpan expiration);
    Task<T?> GetAsync<T>(string key);
    Task RemoveAsync(string key);

    Task AddToListAsync(string key, string value);
    Task RemoveListAsync(string key);
}