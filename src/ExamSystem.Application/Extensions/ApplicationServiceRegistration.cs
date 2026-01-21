using ExamSystem.Application.Common.Behaviors;
using ExamSystem.Application.Settings;
using ExamSystem.Domain;
using FluentValidation;
using MediatR;
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
            services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));
            services.Configure<FrontendURLsSettings>(configuration.GetSection(nameof(FrontendURLsSettings)));
            services.Configure<RefreshTokenSettings>(configuration.GetSection(nameof(RefreshTokenSettings)));

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
