namespace ExamSystem.Application.Common.Results.Errors
{
    public enum ErrorType
    {
        Validation,      // for FluentValidation
        BadRequest,      // for Business Login
        Unauthorized,
        Forbidden,
        NotFound,
        Conflict,
        Failure
    }
}
