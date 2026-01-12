using ExamSystem.Domain.Interfaces;

namespace ExamSystem.API.Extensions
{
    public static class WebApplicationExtensions
    {
        public static async Task InitializeAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
            await initializer.InitializeAsync();
        }
    }
}
