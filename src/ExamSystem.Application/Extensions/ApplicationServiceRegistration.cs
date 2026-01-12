using ExamSystem.Application.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExamSystem.Application.Extensions
{
    public static class ApplicationServiceRegistration
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddApplicationServices(IConfiguration configuration)
            {
                RegisterDependencies(services, configuration);
                return services;
            }
        }
        private static void RegisterDependencies(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DefaultUsersSettings>(configuration.GetSection(nameof(DefaultUsersSettings)));
        }
    }
}
