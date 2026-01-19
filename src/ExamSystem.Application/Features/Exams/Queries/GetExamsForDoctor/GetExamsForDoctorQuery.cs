using ExamSystem.Application.Common.Interfaces;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Features.Exams.DTOs;
using ExamSystem.Domain.Entities.Exams;

namespace ExamSystem.Application.Features.Exams.Queries.GetExamsForDoctor
{
    public record GetExamsForDoctorQuery(
        string DoctorId,
        ExamStatus? ExamStatus,
        int PageNumber,
        int PageSize,
        string BaseUrl,
        Dictionary<string, string> QueryParams)
        : IResultRequest<PaginatedResult<ExamSummaryDto>>,
          IPaginatedQuery;
}
