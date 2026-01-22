using AutoMapper;
using ExamSystem.Application.Features.ExamResults.Queries.GetExamResultsForCurrentStudent.Responses;
using ExamSystem.Application.Features.ExamResults.Queries.GetExamResultsForDoctor.Responses;
using ExamSystem.Application.Features.ExamResults.Queries.GetExamReviewForCurrentStudent.Responses;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Entities.Questions;

namespace ExamSystem.Application.Features.ExamResults.Mapping
{
    public class ExamResultMappingProfile : Profile
    {
        public ExamResultMappingProfile()
        {
            GetExamResultsForDoctorMapper();
            GetExamResultsForCurrentStudentMapper();
            GetExamReviewForCurrentStudentMapper();


        }

        private void GetExamResultsForDoctorMapper()
        {
            CreateMap<ExamResult, StudentExamResultForDoctorResponse>()
                .ForMember(dest => dest.ExamTitle, opts => opts.MapFrom(x => x.Exam.Title))
                .ForMember(dest => dest.StudentDegree, opts => opts.MapFrom(x => x.Score))
                .ForMember(dest => dest.ExamDegree, opts => opts.MapFrom(x => x.TotalMark))
                .ForMember(dest => dest.StudentName, opts => opts.MapFrom(x => x.Student.FullName))
                .ForMember(dest => dest.Status, opts => opts.Ignore())
                .ForMember(dest => dest.Percentage, opts => opts.Ignore());
        }
        private void GetExamResultsForCurrentStudentMapper()
        {
            CreateMap<ExamResult, StudentExamResultForStudentResponse>()
                .ForMember(dest => dest.ExamTitle, opts => opts.MapFrom(x => x.Exam.Title))
                .ForMember(dest => dest.StudentDegree, opts => opts.MapFrom(x => x.Score))
                .ForMember(dest => dest.ExamDegree, opts => opts.MapFrom(x => x.TotalMark))
                .ForMember(dest => dest.ExamDate, opts => opts.MapFrom(x => x.Exam.StartAt))
                .ForMember(dest => dest.Status, opts => opts.Ignore())
                .ForMember(dest => dest.Percentage, opts => opts.Ignore());
        }
        private void GetExamReviewForCurrentStudentMapper()
        {
            CreateMap<ExamResult, ExamReviewResponse>()
                .ForMember(dest => dest.ExamId, opts => opts.MapFrom(x => x.Exam.Id))
                .ForMember(dest => dest.ExamTitle, opts => opts.MapFrom(x => x.Exam.Title))
                .ForMember(dest => dest.ExamDate, opts => opts.MapFrom(x => x.Exam.StartAt))
                .ForMember(dest => dest.Questions, opts => opts.MapFrom(x => x.Exam.Questions))
                .ForMember(dest => dest.StudentDegree, opts => opts.MapFrom(x => x.Score))
                .ForMember(dest => dest.ExamDegree, opts => opts.MapFrom(x => x.TotalMark))
                .ForMember(dest => dest.TotalQuestions, opts => opts.Ignore())
                .ForMember(dest => dest.CorrectAnswers, opts => opts.Ignore())
                .ForMember(dest => dest.Status, opts => opts.Ignore())
                .ForMember(dest => dest.Percentage, opts => opts.Ignore());


            CreateMap<Question, ExamQuestionReviewResponse>()
                .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.StudentOption, opt => opt.Ignore());

        }
    }
}
