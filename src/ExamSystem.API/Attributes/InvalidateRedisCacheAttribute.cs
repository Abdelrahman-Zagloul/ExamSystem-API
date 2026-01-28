using ExamSystem.Application.Contracts.ExternalServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ExamSystem.API.Attributes
{
    public class InvalidateRedisCacheAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var executedContext = await next();
            if (executedContext.Exception != null)
                return;

            if (executedContext.Result is StatusCodeResult statusCodeResult && statusCodeResult.StatusCode >= 400)
                return;

            var cacheService = context.HttpContext.RequestServices.GetService<ICacheService>();
            var cacheKeyPrefix = context.HttpContext.Request.Path;

            if (cacheService != null)
                await cacheService.RemoveByPrefixAsync(cacheKeyPrefix);
        }
    }
}
