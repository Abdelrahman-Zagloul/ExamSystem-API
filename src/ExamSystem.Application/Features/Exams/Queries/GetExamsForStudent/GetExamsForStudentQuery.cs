using ExamSystem.Application.Common.Interfaces;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Features.Exams.DTOs;

namespace ExamSystem.Application.Features.Exams.Queries.GetExamsForStudent
{
    public record GetExamsForStudentQuery
        (
            ExamStatus? ExamStatus,
            int PageNumber,
            int PageSize,
            string BaseUrl)
        : IPaginatedQuery,
        IResultRequest<PaginatedResult<ExamDetailsForStudentDto>>;
}
