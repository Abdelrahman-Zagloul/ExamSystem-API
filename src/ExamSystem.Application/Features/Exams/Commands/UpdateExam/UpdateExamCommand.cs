using ExamSystem.Application.Common.Results;
using MediatR;

namespace ExamSystem.Application.Features.Exams.Commands.UpdateExam
{
    public record UpdateExamCommand(
        int ExamId,
        string? Title,
        string? Description,
        DateTime? StartAt,
        DateTime? EndAt,
        int? DurationInMinutes
        ) : IRequest<Result>;
}