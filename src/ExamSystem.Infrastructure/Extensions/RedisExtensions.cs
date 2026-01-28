using ExamSystem.Application.Contracts.ExternalServices;
using ExamSystem.Infrastructure.ExternalServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace ExamSystem.Infrastructure.Extensions
{
    public static class RedisExtensions
    {
        public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConnection = configuration.GetConnectionString("RedisConnection") ??
                throw new InvalidOperationException("Redis connection string 'RedisConnection' is missing.");

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect(redisConnection, options =>
                {
                    options.AbortOnConnectFail = false;
                    options.ConnectTimeout = 300;
                    options.SyncTimeout = 300;
                });
            });


            services.AddScoped<ICacheService, RedisCacheService>();
            return services;
        }
    }
}
