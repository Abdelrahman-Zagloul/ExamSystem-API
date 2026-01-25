using AutoMapper;
using ExamSystem.Application.Features.ExamResults.Queries.GetExamResultsForCurrentStudent.Responses;
using ExamSystem.Application.Features.ExamResults.Queries.GetExamResultsForDoctor.Responses;
using ExamSystem.Application.Features.ExamResults.Queries.GetExamReviewForCurrentStudent.Responses;
using ExamSystem.Application.Features.Questions.Shared;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Entities.Questions;

namespace ExamSystem.Application.Tests.Features.ExamResults.Mapping
{
    public class TestExamResultMappingProfile : Profile
    {
        public TestExamResultMappingProfile()
        {
            CreateMap<ExamResult, StudentExamResultForDoctorResponse>()
                .ForAllMembers(opt => opt.Ignore());

            CreateMap<ExamResult, ExamReviewResponse>()
                .ForAllMembers(opt => opt.Ignore());

            CreateMap<Question, ExamQuestionReviewResponse>()
                .ForAllMembers(opt => opt.Ignore());

            CreateMap<Option, OptionResponse>()
                .ForAllMembers(opt => opt.Ignore());

            CreateMap<ExamResult, StudentExamResultForStudentResponse>()
                .ForAllMembers(opt => opt.Ignore());
        }
    }
}
