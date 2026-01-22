using ExamSystem.Application.Features.Questions.Shared;
using ExamSystem.Domain.Entities.Questions;

namespace ExamSystem.Application.Features.Questions.Queries.GetExamQuestionsForDoctor.Responses
{
    public class QuestionForDoctorResponse
    {
        public int QuestionId { get; init; }
        public string? QuestionText { get; init; }
        public double QuestionMark { get; init; }
        public QuestionType QuestionType { get; init; }
        public List<OptionResponse>? Options { get; init; }
        public OptionResponse? CorrectOption { get; init; }
        public string? ExamTitle { get; init; }
    }
}
