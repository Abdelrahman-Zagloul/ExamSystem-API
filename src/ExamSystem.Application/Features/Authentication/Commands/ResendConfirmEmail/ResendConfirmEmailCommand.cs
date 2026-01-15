using ExamSystem.Application.Common.Results;
using MediatR;

namespace ExamSystem.Application.Features.Authentication.Commands.ResendConfirmEmail
{
    public record ResendConfirmEmailCommand(string Email) : IRequest<Result>;
}
