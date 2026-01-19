using Microsoft.AspNetCore.Identity;

namespace ExamSystem.Domain.Entities.Users
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = null!;
    }
}
