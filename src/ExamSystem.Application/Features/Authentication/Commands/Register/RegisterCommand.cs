using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Features.Authentication.DTOs;
using MediatR;

namespace ExamSystem.Application.Features.Authentication.Commands.Register
{
    public record RegisterCommand(string FullName, string Email, string Password, string ConfirmPassword, RoleDto Role)
        : IRequest<Result>;
}
