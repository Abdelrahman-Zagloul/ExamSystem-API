
using ExamSystem.API.Extensions;
using ExamSystem.Application;
using ExamSystem.Infrastructure;
using Serilog;

namespace ExamSystem.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog();
            builder.Services.AddApiServices(builder.Configuration)
                .AddApplicationServices()
                .AddInfrastructureServices(builder.Configuration);

            var app = builder.Build();

            await app.UseApiPipeline();

            app.Run();
        }
    }
}
