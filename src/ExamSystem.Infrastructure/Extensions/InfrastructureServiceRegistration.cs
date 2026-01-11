using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace ExamSystem.Infrastructure.Extensions
{
    public static class InfrastructureServiceRegistration
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddInfrastructureServices(IConfiguration configuration)
            {

                return services;
            }
        }
    }
}
