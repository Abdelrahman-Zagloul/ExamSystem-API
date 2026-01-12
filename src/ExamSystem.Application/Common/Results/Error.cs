namespace ExamSystem.Application.Common.Results
{
    public record Error(string Title, string Description, ErrorType ErrorType)
    {
        public static Error Failure(string title = "Failure", string description = "Failure has occurred")
            => new Error(title, description, ErrorType.Failure);
        public static Error NotFound(string title = "NotFound", string description = "Resource not found")
            => new Error(title, description, ErrorType.NotFound);
        public static Error Validation(string title = "Validation Error", string description = "There was a validation error")
            => new Error(title, description, ErrorType.Validation);
        public static Error Unauthorized(string title = "Unauthorized", string description = "You are not authorized to perform this action")
            => new Error(title, description, ErrorType.Unauthorized);
        public static Error Forbidden(string title = "Forbidden", string description = "You do not have permission to access this resource")
            => new Error(title, description, ErrorType.Forbidden);
    }
}
