using ExamSystem.Application.Common.Interfaces;
using ExamSystem.Application.Features.Authentication.DTOs;

namespace ExamSystem.Application.Features.Authentication.Commands.ConfirmEmail
{
    public record ConfirmEmailCommand(string Email, string Token) : IResultRequest<AuthDto>;
}
