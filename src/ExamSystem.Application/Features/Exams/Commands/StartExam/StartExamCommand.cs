using ExamSystem.Application.Common.Interfaces;
using ExamSystem.Application.Features.Exams.Commands.StartExam.Responses;

namespace ExamSystem.Application.Features.Exams.Commands.StartExam
{
    public record StartExamCommand(string StudentId, int ExamId) : IResultRequest<StartExamResponse>;
}
