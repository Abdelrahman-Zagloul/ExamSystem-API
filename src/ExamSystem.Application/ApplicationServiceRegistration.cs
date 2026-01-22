using ExamSystem.Application.Common.Behaviors;
using ExamSystem.Domain;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ExamSystem.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            RegisterDependencies(services);
            ConfigureFluentValidation(services);
            ConfigureAutoMapper(services);
            ConfigureMediatR(services);

            return services;
        }


        private static void RegisterDependencies(IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
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
