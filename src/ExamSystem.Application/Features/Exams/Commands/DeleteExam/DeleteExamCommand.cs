using ExamSystem.Application.Common.Results;
using MediatR;

namespace ExamSystem.Application.Features.Exams.Commands.DeleteExam
{
    public record DeleteExamCommand(int ExamId) : IRequest<Result>;
}
