using AutoMapper;
using ExamSystem.Application.Features.ExamResults.DTOs;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Entities.Questions;

namespace ExamSystem.Application.Mappers
{
    public class ExamResultProfile : Profile
    {
        public ExamResultProfile()
        {
            GetExamResultsForDoctorMapper();
            GetExamResultsForCurrentStudentMapper();
            GetExamResultDetailsForCurrentStudentMapper();


        }

        private void GetExamResultsForDoctorMapper()
        {
            CreateMap<ExamResult, StudentExamResultForDoctorDto>()
                .ForMember(dest => dest.ExamTitle, opts => opts.MapFrom(x => x.Exam.Title))
                .ForMember(dest => dest.StudentDegree, opts => opts.MapFrom(x => x.Score))
                .ForMember(dest => dest.ExamDegree, opts => opts.MapFrom(x => x.TotalMark))
                .ForMember(dest => dest.StudentName, opts => opts.MapFrom(x => x.Student.FullName))
                .ForMember(dest => dest.Status, opts => opts.Ignore())
                .ForMember(dest => dest.Percentage, opts => opts.Ignore());
        }
        private void GetExamResultsForCurrentStudentMapper()
        {
            CreateMap<ExamResult, StudentExamResultForStudentDto>()
                .ForMember(dest => dest.ExamTitle, opts => opts.MapFrom(x => x.Exam.Title))
                .ForMember(dest => dest.StudentDegree, opts => opts.MapFrom(x => x.Score))
                .ForMember(dest => dest.ExamDegree, opts => opts.MapFrom(x => x.TotalMark))
                .ForMember(dest => dest.ExamDate, opts => opts.MapFrom(x => x.Exam.StartAt))
                .ForMember(dest => dest.Status, opts => opts.Ignore())
                .ForMember(dest => dest.Percentage, opts => opts.Ignore());
        }
        private void GetExamResultDetailsForCurrentStudentMapper()
        {
            CreateMap<ExamResult, ExamResultDetailsForCurrentStudent>()
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


            CreateMap<Question, ExamQuestionResultDto>()
                .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.StudentOption, opt => opt.Ignore());

        }
    }
}
