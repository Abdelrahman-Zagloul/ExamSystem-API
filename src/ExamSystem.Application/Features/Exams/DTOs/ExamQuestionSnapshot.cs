namespace ExamSystem.Application.Features.Exams.DTOs
{
    internal record ExamQuestionSnapshot
    (
        int QuestionId,
        int? CorrectOptionId,
        double QuestionMark
    );
}
