using ExamSystem.Application.Contracts.ExternalServices;
using ExamSystem.Application.Contracts.Jobs;
using ExamSystem.Infrastructure.ExternalServices;
using ExamSystem.Infrastructure.Jobs;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExamSystem.Infrastructure.Extensions
{
    public static class HangfireExtensions
    {
        public static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddHangfire(options =>
            {
                options.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseRecommendedSerializerSettings()
                        .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
                        {
                            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                            QueuePollInterval = TimeSpan.Zero,
                            UseRecommendedIsolationLevel = true,
                            DisableGlobalLocks = true
                        });
            });

            services.AddHangfireServer();


            services.AddScoped<IBackgroundJobService, HangfireBackgroundJobService>();
            services.AddScoped<ICalculateExamResultJob, CalculateExamResultJob>();
            services.AddScoped<IPublishExamResultsJob, PublishExamResultsJob>();
            return services;
        }
    }
}
