using ExamSystem.API.Common.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ExamSystem.API.Extensions
{
    public static class JwtExtensions
    {
        public static IServiceCollection AddJwtEvents(this IServiceCollection services)
        {
            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        var response = ApiResponse.Failure("Unauthorized", new ErrorResponse
                        {
                            Title = "Auth.Unauthorized",
                            Description = "Authentication token is missing or invalid"
                        });
                        await context.Response.WriteAsJsonAsync(response);

                    },
                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        var response = ApiResponse.Failure("Forbidden", new ErrorResponse
                        {
                            Title = "Auth.Forbidden",
                            Description = "You do not have permission to access this resource"
                        });
                        await context.Response.WriteAsJsonAsync(response);
                    }
                };
            });
            return services;
        }
    }
}
