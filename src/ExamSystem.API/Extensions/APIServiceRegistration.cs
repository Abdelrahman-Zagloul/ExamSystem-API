using ExamSystem.Application.Settings;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using Serilog.Events;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

namespace ExamSystem.API.Extensions
{
    public static class APIServiceRegistration
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            ConfigureControllersAndSwagger(services);
            ConfigureCorsPolicy(services, configuration);
            ConfigureSerilog();
            ConfigureRateLimiter(services);
            return services;
        }

        private static void ConfigureControllersAndSwagger(IServiceCollection services)
        {
            services.AddControllers()
            .AddJsonOptions(cfg =>
            {
                cfg.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            services.AddSwaggerDocumentation();
        }
        private static void ConfigureCorsPolicy(IServiceCollection services, IConfiguration configuration)
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
        }
        private static void ConfigureSerilog()
        {
            string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message:lj} {Properties}{NewLine}{Exception}";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console
                (
                    outputTemplate: outputTemplate
                )
                .WriteTo.File
                (
                    path: "Logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    fileSizeLimitBytes: 10 * 1024 * 1024,
                    rollOnFileSizeLimit: true,
                    retainedFileCountLimit: 10,
                    shared: true,
                outputTemplate: outputTemplate
                )
                .CreateLogger();
        }
        private static void ConfigureRateLimiter(IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.AddSlidingWindowLimiter("sliding", limiterOptions =>
                {
                    limiterOptions.PermitLimit = 100;
                    limiterOptions.Window = TimeSpan.FromMinutes(1);
                    limiterOptions.SegmentsPerWindow = 6;
                    limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    limiterOptions.QueueLimit = 0;
                });
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

        }
    }
}
