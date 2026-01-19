using AutoMapper;
using ExamSystem.Application.Features.ExamResults.DTOs;
using ExamSystem.Domain.Entities.Exams;

namespace ExamSystem.Application.Mappers
{
    public class ExamResultProfile : Profile
    {
        public ExamResultProfile()
        {
            GetDoctorExamResultsMapper();


        }

        private void GetDoctorExamResultsMapper()
        {
            CreateMap<ExamResult, StudentExamResultForDoctorDto>()
                .ForMember(dest => dest.ExamTitle, opts => opts.MapFrom(x => x.Exam.Title))
                .ForMember(dest => dest.StudentDegree, opts => opts.MapFrom(x => x.Score))
                .ForMember(dest => dest.ExamDegree, opts => opts.MapFrom(x => x.TotalMark))
                .ForMember(dest => dest.StudentName, opts => opts.MapFrom(x => x.Student.FullName))
                .ForMember(dest => dest.Status, opts => opts.Ignore());
        }
    }
}
