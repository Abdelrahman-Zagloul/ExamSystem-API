using ExamSystem.Application.Settings;

namespace ExamSystem.API.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection(nameof(CorsSettings)).Get<CorsSettings>();
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            services.AddCors(options =>
            {
                options.AddPolicy("DevelopmentPolicy", policy =>
                {
                    policy
                    .AllowAnyHeader()
                    .AllowAnyOrigin()
                    .AllowAnyMethod();
                });

                options.AddPolicy("ProductionPolicy", policy =>
                {
                    policy
                    .WithOrigins(settings.AllowedOrigins)
                    .WithHeaders(settings.AllowedHeaders)
                    .WithMethods(settings.AllowedMethods);

                    if (settings.AllowCredentials)
                        policy.AllowCredentials();
                });
            });

            return services;
        }
    }
}
