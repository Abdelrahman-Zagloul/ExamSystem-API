namespace ExamSystem.API.Common.Responses
{
    public class ApiResponse
    {
        public bool IsSuccess { get; init; }
        public string? Message { get; init; }
        public object? Data { get; init; }
        public object? Errors { get; init; }

        private ApiResponse() { }
        public static ApiResponse Success(object? data = null, string? message = null)
            => new ApiResponse()
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };
        public static ApiResponse Failure(string? message, ErrorResponse? error)
            => new ApiResponse()
            {
                IsSuccess = false,
                Message = message,
                Errors = error
            };
        public static ApiResponse ValidationFailure(
            string? message = "One or more validation errors occurred",
            Dictionary<string, List<string>>? errors = null)
            => new ApiResponse()
            {
                IsSuccess = false,
                Message = message,
                Errors = errors
            };
    }
}
