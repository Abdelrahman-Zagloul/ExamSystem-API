using ExamSystem.Application.Contracts.ExternalServices;
using ExamSystem.Application.Contracts.Services;
using ExamSystem.Infrastructure.ExternalServices;
using ExamSystem.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExamSystem.Infrastructure.Extensions
{
    public static class ExternalServicesExtensions
    {
        public static IServiceCollection AddExternalServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IAppEmailService, AppEmailService>();

            return services;
        }
    }
}
