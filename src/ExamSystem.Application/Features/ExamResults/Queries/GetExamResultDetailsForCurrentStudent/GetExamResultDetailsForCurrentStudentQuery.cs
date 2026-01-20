using ExamSystem.Application.Common.Interfaces;
using ExamSystem.Application.Features.ExamResults.DTOs;

namespace ExamSystem.Application.Features.ExamResults.Queries.GetExamResultDetailsForCurrentStudent
{
    public record GetExamResultDetailsForCurrentStudentQuery
        (
            string StudentId,
            int ExamId
        ) : IResultRequest<ExamResultDetailsForCurrentStudent>;
}
