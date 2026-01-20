
using ExamSystem.API.Extensions;
using ExamSystem.Application.Extensions;
using ExamSystem.Infrastructure.Extensions;
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
                .AddApplicationServices(builder.Configuration)
                .AddInfrastructureServices(builder.Configuration);

            var app = builder.Build();

            await app.UseApiPipeline();

            app.Run();
        }
    }
}
