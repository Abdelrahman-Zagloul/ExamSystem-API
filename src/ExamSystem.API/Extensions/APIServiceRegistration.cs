using System.Text.Json.Serialization;

namespace ExamSystem.API.Extensions
{
    public static class APIServiceRegistration
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddApiServices(IConfiguration configuration)
            {
                services.AddControllers()
                    .AddJsonOptions(cfg =>
                    {
                        cfg.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    });
                services.AddOpenApi();

                return services;
            }
        }
    }
}
