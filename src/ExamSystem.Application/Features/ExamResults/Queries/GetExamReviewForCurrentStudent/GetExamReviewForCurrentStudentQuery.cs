using ExamSystem.Application.Common.Interfaces;
using ExamSystem.Application.Features.ExamResults.Queries.GetExamReviewForCurrentStudent.Responses;

namespace ExamSystem.Application.Features.ExamResults.Queries.GetExamReviewForCurrentStudent
{
    public record GetExamReviewForCurrentStudentQuery
        (
            string StudentId,
            int ExamId
        ) : IResultRequest<ExamReviewResponse>;
}
