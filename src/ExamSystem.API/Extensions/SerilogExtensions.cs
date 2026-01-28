using Serilog;
using Serilog.Events;

namespace ExamSystem.API.Extensions
{
    public static class SerilogExtensions
    {
        public static void AddSerilogLogging(this IServiceCollection services)
        {
            string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message:lj} {Properties}{NewLine}{Exception}";

            Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .WriteTo.Console
                    (
                        outputTemplate: outputTemplate
                    )
                    .WriteTo.File
                    (
                        path: "Logs/log-.txt",
                        rollingInterval: RollingInterval.Day,
                        fileSizeLimitBytes: 10 * 1024 * 1024,
                        rollOnFileSizeLimit: true,
                        retainedFileCountLimit: 10,
                        shared: true,
                        outputTemplate: outputTemplate
                    )
                    .CreateLogger();
        }
    }
}
