using ExamSystem.API.Filters;
using Microsoft.OpenApi.Models;

namespace ExamSystem.API.Extensions
{
    public static class SwaggerRegistration
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.OperationFilter<RemoveApiVersionParametersFilter>();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Exam System API",
                    Version = "v1",
                    Description = "REST API for Exam System Application",
                    Contact = new OpenApiContact
                    {
                        Name = "Exam System Support",
                        Email = "abdelrahman.zagloul.dev@gmail.com",
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    },
                });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token like: Bearer {your token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new List<string>()
                    }
                });

                options.EnableAnnotations();
            });
            return services;
        }
    }
}
