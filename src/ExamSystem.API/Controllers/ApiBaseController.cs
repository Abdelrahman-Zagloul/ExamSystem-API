using ExamSystem.API.Common.Factories;
using ExamSystem.Application.Common.Results;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExamSystem.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ApiBaseController : ControllerBase
    {
        protected string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        protected string GetBaseUrl() => $"{Request.Scheme}://{Request.Host}{Request.Path}";
        protected Dictionary<string, string> GetQueryParams()
            => Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());

        protected ActionResult HandleResult(Result result) =>
           ApiResponseFactory.Create(result, this);

        protected ActionResult HandleResult<TValue>(Result<TValue> result) =>
            ApiResponseFactory.Create(result, this);

    }
}
