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
            return controller.StatusCode((int)error.ErrorType,
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
    }
}