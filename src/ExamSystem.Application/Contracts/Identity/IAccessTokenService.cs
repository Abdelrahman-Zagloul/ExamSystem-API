using ExamSystem.Application.Features.Authentication.Shared;
using ExamSystem.Domain.Entities.Users;

namespace ExamSystem.Application.Contracts.Identity
{
    public interface IAccessTokenService
    {
        Task<AccessTokenResponse> GenerateTokenAsync(ApplicationUser user);
    }
}
