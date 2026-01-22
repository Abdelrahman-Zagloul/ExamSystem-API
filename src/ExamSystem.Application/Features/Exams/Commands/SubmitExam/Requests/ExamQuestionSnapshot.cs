namespace ExamSystem.Application.Features.Exams.Commands.SubmitExam.Requests
{
    internal record ExamQuestionSnapshot
    (
        int QuestionId,
        int? CorrectOptionId,
        double QuestionMark
    );
}
