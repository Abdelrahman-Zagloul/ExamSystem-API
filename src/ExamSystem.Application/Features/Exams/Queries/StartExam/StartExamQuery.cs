using ExamSystem.Application.Common.Interfaces;
using ExamSystem.Application.Features.Exams.DTOs;

namespace ExamSystem.Application.Features.Exams.Queries.StartExam
{
    public record StartExamQuery(string StudentId, int ExamId) : IResultRequest<StartExamResponseDto>;
}
