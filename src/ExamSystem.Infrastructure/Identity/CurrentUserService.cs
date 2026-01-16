using ExamSystem.Application.Contracts.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ExamSystem.Infrastructure.Identity
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private ClaimsPrincipal? User => _contextAccessor.HttpContext?.User;
        public CurrentUserService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public string? UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public string? Email => User?.FindFirstValue(ClaimTypes.Email);

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

        public bool IsInRole(string role) => User?.IsInRole(role) ?? false;

        public IEnumerable<string> Roles => User?.Claims
                    .Where(x => x.Type == ClaimTypes.Role)
                    .Select(x => x.Value) ?? [];
    }
}
