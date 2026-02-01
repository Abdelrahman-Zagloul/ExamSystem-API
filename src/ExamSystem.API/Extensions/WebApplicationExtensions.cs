using ExamSystem.API.Middlewares;
using ExamSystem.Domain.Interfaces;
using Hangfire;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace ExamSystem.API.Extensions
{
    public static class WebApplicationExtensions
    {
        public static async Task UseApiPipeline(this WebApplication app)
        {

            app.UseMiddleware<GlobalExceptionMiddleware>();
            ConfigureEnvironmentMiddleware(app);
            ConfigureRequestPipeline(app);
            await app.InitializeAsync();
        }


        private static void ConfigureEnvironmentMiddleware(WebApplication app)
        {
            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.DisplayRequestDuration();
                options.EnableFilter();
                options.DocExpansion(DocExpansion.None);
            });
            //}
        }
        private static void ConfigureRequestPipeline(WebApplication app)
        {
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();

            //if (app.Environment.IsDevelopment())
            app.UseCors("DevelopmentPolicy");
            //else
            //    app.UseCors("ProductionPolicy");

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();
            app.MapControllers()
                .RequireRateLimiting("sliding"); ;

            app.UseHangfireDashboard("/hangfire");
            app.MapHealthChecks();
        }
        private static async Task InitializeAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
            await initializer.InitializeAsync();
        }
    }
}
