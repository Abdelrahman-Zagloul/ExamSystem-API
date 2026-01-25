using ExamSystem.API.Common.Responses;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.API.Common.Factories
{
    public class ApiResponseFactory
    {
        public static ActionResult Create(Result result, ControllerBase controller)
        {
            if (result.IsSuccess)
                return controller.Ok(ApiResponse.Success(message: result.Message));

            return CreateFailure(result, controller);
        }

        public static ActionResult Create<TValue>(Result<TValue> result, ControllerBase controller)
        {
            if (result.IsSuccess)
                return controller.Ok(ApiResponse.Success(result.Value, result.Message));

            return CreateFailure(result, controller);
        }

        private static ActionResult CreateFailure(Result result, ControllerBase controller)
        {
            if (result.Errors.All(e => e.ErrorType == ErrorType.Validation))
                return HandleValidationErrors(result, controller);

            var error = result.Errors.First();
            var statusCode = ToStatusCode(error.ErrorType);
            return controller.StatusCode(statusCode,
                ApiResponse.Failure(result.Message, new ErrorResponse
                {
                    Title = error.Title,
                    Description = error.Description
                }));
        }

        private static ActionResult HandleValidationErrors(Result result, ControllerBase controller)
        {
            var validationErrors = result.Errors
                    .GroupBy(e => e.Title)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToList());
            return controller.BadRequest(ApiResponse.ValidationFailure(errors: validationErrors));
        }

        private static int ToStatusCode(ErrorType errorType)
        {
            return errorType switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.BadRequest => StatusCodes.Status400BadRequest,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Failure => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError
            };
        }
    }
}