
using ExamSystem.API.Extensions;
using ExamSystem.Application.Extensions;
using ExamSystem.Infrastructure.Extensions;
using Hangfire;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace ExamSystem.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddApiServices(builder.Configuration)
                .AddApplicationServices(builder.Configuration)
                .AddInfrastructureServices(builder.Configuration);

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.DisplayRequestDuration();
                options.EnableFilter();
                options.DocExpansion(DocExpansion.None);
            });

            await app.InitializeAsync();
            app.UseHangfireDashboard("/hangfire");

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }


    }
}
