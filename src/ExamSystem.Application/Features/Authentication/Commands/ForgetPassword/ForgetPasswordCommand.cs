using ExamSystem.Application.Common.Results;
using MediatR;

namespace ExamSystem.Application.Features.Authentication.Commands.ForgetPassword
{
    public record ForgetPasswordCommand(string Email) : IRequest<Result>;
}
