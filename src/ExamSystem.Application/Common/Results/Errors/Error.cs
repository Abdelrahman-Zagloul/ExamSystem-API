namespace ExamSystem.Application.Common.Results.Errors
{
    public record Error(string Title, string Description, ErrorType ErrorType)
    {
        public string Code { get; set; }

        public static Error BadRequest(string title = "BadRequest", string description = "The request is invalid")
            => new Error(title, description, ErrorType.BadRequest);
        public static Error Validation(string title = "Validation Error", string description = "One or more validation errors occurred")
            => new Error(title, description, ErrorType.Validation);
        public static Error Unauthorized(string title = "Unauthorized", string description = "You are not authorized to perform this action")
            => new Error(title, description, ErrorType.Unauthorized);
        public static Error Forbidden(string title = "Forbidden", string description = "You do not have permission to access this resource")
            => new Error(title, description, ErrorType.Forbidden);
        public static Error NotFound(string title = "NotFound", string description = "Resource not found")
            => new Error(title, description, ErrorType.NotFound);
        public static Error Conflict(string title = "Conflict", string description = "The request conflicts with the current state of the resource")
            => new Error(title, description, ErrorType.Conflict);
        public static Error Failure(string title = "Failure", string description = "Failure has occurred")
            => new Error(title, description, ErrorType.Failure);
    }
}
