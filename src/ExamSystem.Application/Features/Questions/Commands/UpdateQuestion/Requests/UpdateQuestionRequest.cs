namespace ExamSystem.Application.Features.Questions.Commands.UpdateQuestion.Requests
{
    public record UpdateQuestionRequest
    (
        string? QuestionText,
        int? NewQuestionMark,
        List<UpdateOptionRequest>? Options,
        int? NewCorrectOptionId
    );

}
