using ExamSystem.Application.Common.Interfaces;
using ExamSystem.Application.Features.Questions.Shared;

namespace ExamSystem.Application.Features.Questions.Queries.GetQuestionById
{
    public record GetQuestionByIdQuery(string DoctorId, int ExamId, int QuestionId) : IResultRequest<QuestionResponse>;
}
