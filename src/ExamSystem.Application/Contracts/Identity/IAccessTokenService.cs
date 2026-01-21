using ExamSystem.Application.Features.Authentication.DTOs;
using ExamSystem.Domain.Entities.Users;

namespace ExamSystem.Application.Contracts.Identity
{
    public interface IAccessTokenService
    {
        Task<AccessTokenDto> GenerateTokenAsync(ApplicationUser user);
    }
}
