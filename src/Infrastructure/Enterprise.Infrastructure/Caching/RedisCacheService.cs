using System.Text.Json;
using Enterprise.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Enterprise.Infrastructure.Caching;

/// <summary>
/// Implementation of ICacheService using IDistributedCache (e.g. Redis).
/// Implements the Cache-Aside pattern.
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisCacheService> _logger;
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        try
        {
            var cachedBytes = await _cache.GetAsync(key, cancellationToken);
            if (cachedBytes is null || cachedBytes.Length == 0)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(cachedBytes, DefaultOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting cache key '{CacheKey}'", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        if (value is null)
            return;

        try
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(value, DefaultOptions);
            var options = new DistributedCacheEntryOptions();

            if (expiration.HasValue)
            {
                options.SetAbsoluteExpiration(expiration.Value);
            }
            else
            {
                // Default absolute expiration if none provided
                options.SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
            }

            await _cache.SetAsync(key, bytes, options, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while setting cache key '{CacheKey}'", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        try
        {
            await _cache.RemoveAsync(key, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while removing cache key '{CacheKey}'", key);
        }
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(factory);

        var cachedValue = await GetAsync<T>(key, cancellationToken);
        if (cachedValue is not null)
        {
            return cachedValue;
        }

        var value = await factory(cancellationToken);
        if (value is not null)
        {
            await SetAsync(key, value, expiration, cancellationToken);
        }

        return value;
    }
}
