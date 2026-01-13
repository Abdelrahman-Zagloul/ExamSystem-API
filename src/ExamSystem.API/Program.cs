
using ExamSystem.API.Extensions;
using ExamSystem.Application.Extensions;
using ExamSystem.Infrastructure.Extensions;
using Hangfire;

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


            await app.InitializeAsync();
            app.UseHangfireDashboard("/hangfire");

            app.UseStaticFiles();
            app.MapOpenApi();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }


    }
}
