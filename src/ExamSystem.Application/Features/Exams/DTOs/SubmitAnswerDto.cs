namespace ExamSystem.Application.Features.Exams.DTOs
{
    public class SubmitAnswerDto
    {
        public int QuestionId { get; init; }
        public int SelectedOptionId { get; init; }
    }
}
