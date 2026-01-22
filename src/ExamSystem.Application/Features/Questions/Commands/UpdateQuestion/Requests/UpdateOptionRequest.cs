namespace ExamSystem.Application.Features.Questions.Commands.UpdateQuestion.Requests
{
    public record UpdateOptionRequest(int OptionId, string? NewOptionText);
}
