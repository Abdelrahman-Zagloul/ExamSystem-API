namespace ExamSystem.Application.Features.Exams.Commands.SubmitExam.Requests
{
    public class SubmitAnswerRequest
    {
        public int QuestionId { get; init; }
        public int SelectedOptionId { get; init; }
    }
}
