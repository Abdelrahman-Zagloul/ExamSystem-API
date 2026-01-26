using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ExamSystem.API.Filters
{
    public class RemoveApiVersionParametersFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                return;

            operation.Parameters = operation.Parameters
                .Where(p => !p.Name.Equals("api-version", StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}
