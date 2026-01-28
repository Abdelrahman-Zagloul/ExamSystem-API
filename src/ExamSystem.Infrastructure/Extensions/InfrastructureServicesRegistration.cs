using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExamSystem.Infrastructure.Extensions
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddPersistence(configuration)
                .AddIdentityAndJwt(configuration)
                .AddHangfire(configuration)
                .AddRedis(configuration)
                .AddExternalServices(configuration)
                .AddInfrastructureSettings(configuration);

            return services;
        }
    }
}
