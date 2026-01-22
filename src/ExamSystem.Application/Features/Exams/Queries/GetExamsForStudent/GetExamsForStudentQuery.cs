using ExamSystem.Application.Common.Interfaces;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Features.Exams.Queries.GetExamsForStudent.Responses;
using ExamSystem.Domain.Entities.Exams;

namespace ExamSystem.Application.Features.Exams.Queries.GetExamsForStudent
{
    public record GetExamsForStudentQuery
        (
            ExamStatus? ExamStatus,
            int PageNumber,
            int PageSize,
            string BaseUrl,
            Dictionary<string, string> QueryParams)
        : IPaginatedQuery,
        IResultRequest<PaginatedResult<ExamDetailsForStudentResponse>>;
}
