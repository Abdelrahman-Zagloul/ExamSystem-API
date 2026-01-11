namespace ExamSystem.API.Extensions
{
    public static class APIServiceRegistration
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddApiServices(IConfiguration configuration)
            {
                services.AddControllers();
                services.AddOpenApi();

                return services;
            }
        }
    }
}
