using ExamSystem.Application.Contracts.ExternalServices;
using StackExchange.Redis;
using System.Text.Json;

namespace ExamSystem.Infrastructure.ExternalServices
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _database;
        private readonly IConnectionMultiplexer _redis;
        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _database = redis.GetDatabase();
        }
        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var value = await _database.StringGetAsync(key);
                if (value.IsNullOrEmpty)
                    return default;
                return JsonSerializer.Deserialize<T>(value.ToString());
            }
            catch
            {
                return default;
            }
        }
        public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            try
            {
                await _database.StringSetAsync(key, JsonSerializer.Serialize(value), expiration);
            }
            catch { }
        }
        public async Task RemoveAsync(string key)
        {
            try
            {
                await _database.KeyDeleteAsync(key);
            }
            catch { }
        }
        public async Task RemoveByPrefixAsync(string prefix)
        {
            try
            {
                var endpoints = _redis.GetEndPoints();
                var server = _redis.GetServer(endpoints.First());
                if (server == null || !server.IsConnected)
                    return;

                var keys = server.Keys(pattern: $"{prefix}*").ToArray();

                foreach (var key in keys)
                    await _database.KeyDeleteAsync(key);
            }
            catch { }
        }
    }
}
