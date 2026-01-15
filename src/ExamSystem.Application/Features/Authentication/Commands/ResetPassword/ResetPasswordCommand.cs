using ExamSystem.Application.Common.Results;
using MediatR;

namespace ExamSystem.Application.Features.Authentication.Commands.ResetPassword
{
    public record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest<Result>;
}
