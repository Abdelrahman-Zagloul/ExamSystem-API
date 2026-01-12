using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Features.Authentication.DTOs;
using MediatR;

namespace ExamSystem.Application.Features.Authentication.Commands.Login
{
    public record LoginCommand(string Email, string Password) : IRequest<Result<AuthDto>>;
}
