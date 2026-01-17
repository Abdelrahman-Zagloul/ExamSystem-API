using AutoMapper;
using ExamSystem.Application.Features.Exams.Commands.CreateExam;
using ExamSystem.Application.Features.Exams.Commands.UpdateExam;
using ExamSystem.Domain.Entities;

namespace ExamSystem.Application.Mappers
{
    public class ExamProfile : Profile
    {
        public ExamProfile()
        {
            CreateExamMapper();
            UpdateExamMapper();
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
    }
}
