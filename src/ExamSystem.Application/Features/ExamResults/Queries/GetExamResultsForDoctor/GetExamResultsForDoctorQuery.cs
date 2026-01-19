using ExamSystem.Application.Common.Interfaces;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Features.ExamResults.DTOs;
using ExamSystem.Domain.Entities.Exams;

namespace ExamSystem.Application.Features.ExamResults.Queries.GetExamResultsForDoctor
{
    public record GetExamResultsForDoctorQuery
        (
            string DoctorId,
            int ExamId,
            ExamResultStatus? Status,
            int PageNumber,
            int PageSize,
            string BaseUrl,
            Dictionary<string, string> QueryParams
        ) : IResultRequest<PaginatedResult<StudentExamResultForDoctorDto>>, IPaginatedQuery;
}
