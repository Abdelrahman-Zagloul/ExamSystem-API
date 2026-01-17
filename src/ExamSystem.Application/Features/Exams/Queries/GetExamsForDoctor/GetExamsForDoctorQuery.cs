using ExamSystem.Application.Common.Interfaces;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Features.Exams.DTOs;

namespace ExamSystem.Application.Features.Exams.Queries.GetExamsForDoctor
{
    public record GetExamsForDoctorQuery(
        string DoctorId,
        ExamStatus? ExamStatus,
        int PageNumber,
        int PageSize,
        string BaseUrl)
        : IResultRequest<PaginatedResult<ExamSummaryDto>>,
          IPaginatedQuery;
}
