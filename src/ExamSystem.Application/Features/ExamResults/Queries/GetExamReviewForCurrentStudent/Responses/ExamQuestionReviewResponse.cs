using ExamSystem.Application.Features.Questions.Shared;
using ExamSystem.Domain.Entities.Questions;

namespace ExamSystem.Application.Features.ExamResults.Queries.GetExamReviewForCurrentStudent.Responses
{
    public class ExamQuestionReviewResponse
    {
        public int QuestionId { get; init; }
        public string QuestionText { get; init; } = null!;
        public double QuestionMark { get; init; }
        public QuestionType QuestionType { get; init; }
        public List<OptionResponse> Options { get; init; } = [];
        public OptionResponse CorrectOption { get; init; } = null!;
        public OptionResponse? StudentOption { get; set; }
        public bool IsCorrect => CorrectOption.OptionId == StudentOption?.OptionId;
    }
}
