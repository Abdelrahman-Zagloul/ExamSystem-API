using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Features.Exams.Commands.SubmitExam.Requests;
using MediatR;

namespace ExamSystem.Application.Features.Exams.Commands.SubmitExam
{
    public record SubmitExamCommand(
        string StudentId,
        int ExamId,
        List<SubmitAnswerRequest> Answers
        ) : IRequest<Result>;
}
