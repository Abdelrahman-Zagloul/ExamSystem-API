using ExamSystem.Application.Common.Results;
using MediatR;

namespace ExamSystem.Application.Features.Exams.Commands.CreateExam
{
    public record CreateExamCommand(
        string Title,
        string? Description,
        DateTime StartAt,
        DateTime EndAt,
        int DurationInMinutes
        ) : IRequest<Result>;
}