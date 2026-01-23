using ExamSystem.Application.Contracts.ExternalServices;
using StackExchange.Redis;
using System.Text.Json;

namespace ExamSystem.Infrastructure.ExternalServices
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _database;
        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }
        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _database.StringGetAsync(key);
            if (value.IsNullOrEmpty)
                return default;
            return JsonSerializer.Deserialize<T>(value.ToString());
        }
        public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
            => await _database.StringSetAsync(key, JsonSerializer.Serialize(value), expiration);

        public async Task RemoveAsync(string key)
            => await _database.KeyDeleteAsync(key);

    }
}
