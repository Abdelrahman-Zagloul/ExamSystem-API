using AutoMapper;
using ExamSystem.Application.Features.ExamResults.DTOs;
using ExamSystem.Domain.Entities.Exams;

namespace ExamSystem.Application.Mappers
{
    public class ExamResultProfile : Profile
    {
        public ExamResultProfile()
        {
            GetExamResultsForDoctorMapper();
            GetExamResultsForCurrentStudentMapper();


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
    }
}
