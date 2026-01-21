using ExamSystem.Application.Common.Interfaces;
using ExamSystem.Application.Features.Authentication.DTOs;

namespace ExamSystem.Application.Features.Authentication.Commands.Login
{
    public record LoginCommand
        (
            string Email,
            string Password,
            string? IpAddress
        ) : IResultRequest<AccessWithRefreshTokenDto>;
}
