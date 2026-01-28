namespace ExamSystem.API.Extensions
{
    public static class APIServiceRegistration
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddControllersConfiguration()
                .AddSwaggerDocumentation()
                .AddCorsPolicy(configuration)
                .AddRateLimiter()
                .AddJwtEvents()
                .AddVersioning()
                .AddHealthChecks(configuration);

            services.AddSerilogLogging();

            return services;
        }
    }
}
