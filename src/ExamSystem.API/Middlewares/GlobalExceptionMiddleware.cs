namespace ExamSystem.API.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (!context.Response.HasStarted)
                    await HandleExceptionAsync(context, ex);
                else
                    _logger.LogError(ex, "Unhandled exception after response started. TraceId: {TraceId}", context.TraceIdentifier);
            }
            finally
            {
                _logger.LogInformation("==================== END REQUEST | {Method} {Path} ====================\n\n",
                     context.Request.Method,
                     context.Request.Path);
            }

        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception occurred. TraceId: {TraceId}", context.TraceIdentifier);

            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var response = new
            {
                isSuccess = false,
                message = "Internal Server Error.",
                traceId = context.TraceIdentifier
            };
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
