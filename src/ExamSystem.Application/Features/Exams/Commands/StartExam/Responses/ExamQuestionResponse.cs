using ExamSystem.Application.Features.Questions.Shared;
using ExamSystem.Domain.Entities.Questions;

namespace ExamSystem.Application.Features.Exams.Commands.StartExam.Responses
{
    public record ExamQuestionResponse
    {
        public int QuestionId { get; init; }
        public string QuestionText { get; init; } = null!;
        public double QuestionMark { get; init; }
        public QuestionType QuestionType { get; init; }
        public List<OptionResponse> Options { get; init; } = [];
    }
}
