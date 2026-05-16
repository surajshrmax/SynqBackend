using StackExchange.Redis;
using Synq.Application.Common.Interfaces;

namespace Synq.Infrastructure.Behaviours;

public class CacheService : ICacheService
{
    private readonly IDatabase db;

    public CacheService(IConnectionMultiplexer redis)
    {
        this.db = redis.GetDatabase();
    }

    public async Task<string?> GetValueAsync(string key)
    {
        return await db.StringGetAsync(key);
    }

    public async Task SetValueAsync(string key, string value)
    {
        await db.StringSetAsync(key, value);
    }
}