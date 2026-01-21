namespace ExamSystem.Infrastructure.Identity.Responses
{
    internal static class JwtAuthErrorResponse
    {
        public static object Unauthorized()
        {
            return new
            {
                IsSuccess = false,
                Message = "Unauthorized",
                Data = (object?)null,
                Errors = new
                {
                    Title = "Auth.Unauthorized",
                    Description = "Authentication token is missing or invalid"
                }
            };
        }
        public static object Forbidden()
        {
            return new
            {
                IsSuccess = false,
                Message = "Forbidden",
                Data = (object?)null,
                Errors = new
                {
                    Title = "Auth.Forbidden",
                    Description = "You do not have permission to access this resource"
                }
            };
        }
    }
}
