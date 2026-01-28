using System.Text.Json.Serialization;

namespace ExamSystem.API.Extensions
{
    public static class ControllersExtensions
    {
        public static IServiceCollection AddControllersConfiguration(this IServiceCollection services)
        {
            services.AddControllers()
            .AddJsonOptions(cfg =>
            {
                cfg.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            return services;
        }
    }
}
