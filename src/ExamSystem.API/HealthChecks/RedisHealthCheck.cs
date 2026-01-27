using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace ExamSystem.API.HealthChecks
{
    public class RedisHealthCheck : IHealthCheck
    {
        private readonly IDatabase _database;

        public RedisHealthCheck(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var pong = await _database.PingAsync();
                return HealthCheckResult.Healthy($"Redis responded in {pong.TotalMilliseconds} ms");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Degraded("Redis is unavailable", ex);
            }
        }
    }
}
