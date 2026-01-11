using Microsoft.AspNetCore.Identity;

namespace ExamSystem.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = null!;
    }
}
