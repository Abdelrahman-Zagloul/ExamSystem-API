using ExamSystem.Application.Common.Interfaces;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Features.Questions.DTOs;

namespace ExamSystem.Application.Features.Questions.Queries.GetAllQuestionsForDoctor
{
    public record GetExamQuestionsForDoctorQuery(
            string DoctorId,
            int ExamId,
            int PageNumber,
            int PageSize,
            string BaseUrl)
            : IResultRequest<PaginatedResult<QuestionForDoctorDto>>, IPaginatedQuery;
}
