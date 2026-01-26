using AutoMapper;
using ExamSystem.Application.Features.Exams.Commands.StartExam.Responses;
using ExamSystem.Application.Features.Exams.Queries.GetExamByIdForDoctor.Responses;
using ExamSystem.Application.Features.Exams.Queries.GetExamsForDoctor.Responses;
using ExamSystem.Application.Features.Exams.Queries.GetExamsForStudent.Responses;
using ExamSystem.Application.Features.Questions.Shared;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Entities.Questions;

namespace ExamSystem.Application.Tests.Features.Exams.Mapping
{
    internal class TestExamMappingProfile : Profile
    {
        public TestExamMappingProfile()
        {
            CreateMap<Exam, StartExamResponse>();
            CreateMap<Question, ExamQuestionResponse>();
            CreateMap<Option, OptionResponse>();
            CreateMap<Exam, ExamDetailsForDoctorResponse>();
            CreateMap<Exam, ExamDetailsForStudentResponse>();
            CreateMap<Exam, ExamSummaryResponse>();
        }
    }
}
