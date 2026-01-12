using ExamSystem.Application.Settings;
using ExamSystem.Domain;
using FluentValidation;
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
                ConfigureFluentValidation(services);
                ConfigureAutoMapper(services);
                ConfigureMediatR(services);

                return services;
            }
        }

        private static void RegisterDependencies(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DefaultUsersSettings>(configuration.GetSection(nameof(DefaultUsersSettings)));
            services.Configure<JWTSettings>(configuration.GetSection(nameof(JWTSettings)));
        }
        private static void ConfigureFluentValidation(IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(typeof(IApplicationAssemblyMarker).Assembly);
        }
        private static void ConfigureAutoMapper(IServiceCollection services)
        {
            services.AddAutoMapper(cfg => { }, typeof(IApplicationAssemblyMarker).Assembly, typeof(IDomainAssemblyMarker).Assembly);
        }
        private static void ConfigureMediatR(IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(IApplicationAssemblyMarker).Assembly);
            });
        }
    }
}
