using ExamSystem.Application.Contracts.ExternalServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace ExamSystem.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RedisCacheAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeInMinutes;
        public RedisCacheAttribute(int timeInMinutes)
        {
            _timeInMinutes = timeInMinutes;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
            var cacheKey = GenerateCacheKey(context.HttpContext.Request);
            var cachedValue = await cacheService.GetAsync<object>(cacheKey);

            if (cachedValue != null)
            {
                context.Result = new OkObjectResult(cachedValue);
                return;
            }

            var executedContext = await next();
            if (executedContext.Result is OkObjectResult okObjectResult)
                await cacheService.SetAsync(cacheKey, okObjectResult.Value, TimeSpan.FromMinutes(_timeInMinutes));
        }

        private string GenerateCacheKey(HttpRequest request)
        {
            StringBuilder keyBuilder = new StringBuilder();
            keyBuilder.Append(request.Path);
            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
                keyBuilder.Append($"|{key}:{value}");
            return keyBuilder.ToString();
        }
    }
}
