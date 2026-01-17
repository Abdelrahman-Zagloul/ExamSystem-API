using ExamSystem.Application.Common.Interfaces;
using ExamSystem.Application.Features.Exams.DTOs;

namespace ExamSystem.Application.Features.Exams.Queries.GetExamByIdForDoctor
{
    public record GetExamByIdForDoctorQuery(string DoctorId, int ExamId) : IResultRequest<ExamDetailsForDoctorDto>;
}
