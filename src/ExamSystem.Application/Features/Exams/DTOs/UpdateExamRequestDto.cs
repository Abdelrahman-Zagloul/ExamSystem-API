
namespace ExamSystem.Application.Features.Exams.DTOs
{
    public record UpdateExamRequestDto(
        string? Title,
        string? Description,
        DateTime? StartAt,
        DateTime? EndAt,
        int? DurationInMinutes);
}
