using ExamSystem.Application.Common.Results;

namespace ExamSystem.Application.Features.Authentication.Commands.ResetPassword
{
    public record ResetPasswordCommand(string Email, string Token, string NewPassword) : IResultRequest<string>;
}
