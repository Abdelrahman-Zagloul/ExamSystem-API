using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;

namespace ExamSystem.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ApiBaseController : ControllerBase
    {
        protected string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
        protected string GetBaseUrl() => $"{Request.Scheme}://{Request.Host}{Request.Path}";
        protected ActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
                return Ok(result);
            else
                return HandleResultErrors(result);
        }
        protected ActionResult HandleResult<TValue>(Result<TValue> result)
        {
            if (result.IsSuccess)
                return Ok(result);
            else
                return HandleResultErrors(result);
        }
        private ActionResult HandleResultErrors(Result result)
        {
            if (result.Errors.All(x => x.ErrorType == ErrorType.Validation))
                return HandleValidationErrors(result);

            var statusCode = (int)result.Errors[0].ErrorType;
            return StatusCode(statusCode, new
            {
                isSuccess = false,
                message = result.Message,
                errors = new
                {
                    result.Errors[0].Title,
                    result.Errors[0].Description,
                }
            });
        }
        private ActionResult HandleValidationErrors(Result result)
        {
            var modelState = new ModelStateDictionary();
            foreach (var error in result.Errors)
                modelState.AddModelError(error.Title, error.Description);


            return BadRequest(new
            {
                isSuccess = false,
                message = "One or more validation fail",
                errors = result.Errors.GroupBy(e => e.Title)
                        .ToDictionary(g => g.Key, g => g.Select(x => x.Description).ToList())
            });
        }
    }
}
