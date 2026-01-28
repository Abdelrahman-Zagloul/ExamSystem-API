using ExamSystem.Application.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExamSystem.Infrastructure.Extensions
{
    public static class SettingsExtensions
    {
        public static IServiceCollection AddInfrastructureSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DefaultUsersSettings>(configuration.GetSection(nameof(DefaultUsersSettings)));
            services.Configure<JWTSettings>(configuration.GetSection(nameof(JWTSettings)));
            services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));
            services.Configure<FrontendURLsSettings>(configuration.GetSection(nameof(FrontendURLsSettings)));
            services.Configure<RefreshTokenSettings>(configuration.GetSection(nameof(RefreshTokenSettings)));

            return services;
        }
    }
}
