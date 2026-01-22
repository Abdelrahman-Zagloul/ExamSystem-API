using ExamSystem.Application.Common.Interfaces;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Features.Questions.Shared;

namespace ExamSystem.Application.Features.Questions.Queries.GetQuestionsByExamId
{
    public record GetQuestionsByExamIdQuery(
            string DoctorId,
            int ExamId,
            int PageNumber,
            int PageSize,
            string BaseUrl,
            Dictionary<string, string> QueryParams)
            : IResultRequest<PaginatedResult<QuestionResponse>>, IPaginatedQuery;
}
