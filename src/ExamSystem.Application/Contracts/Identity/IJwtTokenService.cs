using ExamSystem.Application.Features.Authentication.DTOs;
using ExamSystem.Domain.Entities;

namespace ExamSystem.Application.Contracts.Identity
{
    public interface IJwtTokenService
    {
        Task<JwtTokenDto> GenerateTokenAsync(ApplicationUser user);
    }
}
