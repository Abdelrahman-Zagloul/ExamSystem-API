using AutoMapper;
using ExamSystem.Application.Features.Exams.Commands.CreateExam;
using ExamSystem.Application.Features.Exams.Commands.UpdateExam;
using ExamSystem.Application.Features.Exams.DTOs;
using ExamSystem.Domain.Entities;
using System.Linq.Expressions;

namespace ExamSystem.Application.Mappers
{
    public class ExamProfile : Profile
    {
        public ExamProfile()
        {
            CreateExamMapper();
            UpdateExamMapper();
            GetExamsForDoctorMapper();
            GetExamByIdForDoctorMapper();
        }

        private void CreateExamMapper()
        {
            CreateMap<CreateExamCommand, Exam>();
        }
        private void UpdateExamMapper()
        {
            CreateMap<UpdateExamCommand, Exam>()
                .ForMember(x => x.Id, opts => opts.Ignore())
                .ForMember(d => d.Title, o => o.Condition(src => !string.IsNullOrEmpty(src.Title)))
                .ForMember(d => d.Description, o => o.Condition(src => !string.IsNullOrEmpty(src.Description)))
                .ForMember(d => d.StartAt, o => o.Condition(src => src.StartAt.HasValue))
                .ForMember(d => d.EndAt, o => o.Condition(src => src.EndAt.HasValue))
                .ForMember(d => d.DurationInMinutes, o => o.Condition(src => src.DurationInMinutes.HasValue));
        }
        private void GetExamsForDoctorMapper()
        {
            CreateMap<Exam, ExamSummaryDto>()
                .ForMember(dest => dest.ExamStatus, opts => opts.MapFrom(GetExamStatusExpression()))
                .ForMember(dest => dest.QuestionsCount, opts => opts.MapFrom(src => src.Questions.Count))
                .ForMember(dest => dest.SubmissionsCount, opts => opts.MapFrom(src => src.ExamResults.Count));
        }
        private void GetExamByIdForDoctorMapper()
        {
            CreateMap<Exam, ExamDetailsForDoctorDto>()
                .ForMember(dest => dest.ExamStatus, opts => opts.MapFrom(GetExamStatusExpression()))
                .ForMember(dest => dest.QuestionsCount, opts => opts.MapFrom(src => src.Questions.Count))
                .ForMember(dest => dest.SubmissionsCount, opts => opts.MapFrom(src => src.ExamResults.Count))
                .ForMember(dest => dest.StudentsPassCount, opts => opts.MapFrom(src => src.ExamResults.Count(x => x.Score >= (x.TotalMark / 2))))
                .ForMember(dest => dest.StudentsPassCount, opts => opts.MapFrom(src => src.ExamResults.Count(x => x.Score < (x.TotalMark / 2))));
        }
        private Expression<Func<Exam, ExamStatus>> GetExamStatusExpression()
        {
            return x =>
                x.StartAt > DateTime.UtcNow
                    ? ExamStatus.Upcoming
                    : x.StartAt < DateTime.UtcNow && x.EndAt > DateTime.UtcNow
                    ? ExamStatus.Active
                    : ExamStatus.Finished;
        }
    }

}
