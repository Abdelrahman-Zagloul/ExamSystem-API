namespace ExamSystem.Application.Features.Questions.DTOs
{
    public record UpdateQuestionRequestDto
    (
        string? QuestionText,
        int? NewQuestionMark,
        List<UpdateOptionDto>? Options,
        int? NewCorrectOptionId
    );

}
