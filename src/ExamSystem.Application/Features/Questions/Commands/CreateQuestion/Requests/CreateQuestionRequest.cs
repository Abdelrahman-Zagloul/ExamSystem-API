using ExamSystem.Domain.Entities.Questions;

namespace ExamSystem.Application.Features.Questions.Commands.CreateQuestion.Requests
{
    public record CreateQuestionRequest
     (
        string QuestionText,
        double QuestionMark,
        QuestionType QuestionType,
        List<CreateOptionRequest> Options,
        int CorrectOptionNumber
     );
}
