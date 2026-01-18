using Microsoft.AspNetCore.Http;

namespace ExamSystem.Application.Common.Results.Errors
{
    public enum ErrorType
    {
        Validation = StatusCodes.Status400BadRequest,      // for FluentValidation
        BadRequest = StatusCodes.Status400BadRequest,      // for Business Login
        Unauthorized = StatusCodes.Status401Unauthorized,
        Forbidden = StatusCodes.Status403Forbidden,
        NotFound = StatusCodes.Status404NotFound,
        Conflict = StatusCodes.Status409Conflict,
        Failure = StatusCodes.Status500InternalServerError,
    }

}
