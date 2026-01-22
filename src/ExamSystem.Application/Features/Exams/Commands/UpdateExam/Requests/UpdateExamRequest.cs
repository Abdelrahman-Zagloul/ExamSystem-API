namespace ExamSystem.Application.Features.Exams.Commands.UpdateExam.Requests
{
    public record UpdateExamRequest(
        string? Title,
        string? Description,
        DateTime? StartAt,
        DateTime? EndAt,
        int? DurationInMinutes);
}
