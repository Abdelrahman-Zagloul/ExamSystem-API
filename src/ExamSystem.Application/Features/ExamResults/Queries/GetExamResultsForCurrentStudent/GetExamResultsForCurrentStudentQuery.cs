using ExamSystem.Application.Common.Interfaces;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Features.ExamResults.Queries.GetExamResultsForCurrentStudent.Responses;

namespace ExamSystem.Application.Features.ExamResults.Queries.GetExamResultsForCurrentStudent
{
    public record GetExamResultsForCurrentStudentQuery
    (
        string StudentId,
        int PageNumber,
        int PageSize,
        string BaseUrl,
        Dictionary<string, string> QueryParams
    ) : IResultRequest<PaginatedResult<StudentExamResultForStudentResponse>>, IPaginatedQuery;
}
