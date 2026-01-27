using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExamSystem.API.Tests.Helpers
{
    public static class ClaimsHelper
    {

        public static void SetUser(this ControllerBase _controller, string userId, string role)
        {
            _controller.HttpContext.User =
                new ClaimsPrincipal(
                    new ClaimsIdentity(
                        new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, userId),
                            new Claim(ClaimTypes.Role, role)
                        },
                        "TestAuth"
                    )
                );
        }
    }
}
