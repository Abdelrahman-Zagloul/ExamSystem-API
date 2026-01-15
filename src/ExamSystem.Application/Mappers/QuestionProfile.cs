using AutoMapper;
using ExamSystem.Application.Features.Questions.Commands.CreateQuestion;
using ExamSystem.Application.Features.Questions.Commands.UpdateQuestion;
using ExamSystem.Application.Features.Questions.DTOs;
using ExamSystem.Domain.Entities;

namespace ExamSystem.Application.Mappers
{
    public class QuestionProfile : Profile
    {
        public QuestionProfile()
        {
            CreateMap<CreateQuestionCommand, Question>()
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options))
                .ForMember(dest => dest.CorrectOptionId, opt => opt.Ignore());

            CreateMap<CreateOptionDto, Option>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());



            CreateMap<UpdateQuestionCommand, Question>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.QuestionId))
                .ForMember(dest => dest.QuestionMark, opts => opts.MapFrom(src => src.NewQuestionMark))
                .ForMember(dest => dest.CorrectOptionId, opts => opts.MapFrom(src => src.NewCorrectOptionId))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateOptionDto, Option>()
                .ForMember(dest => dest.OptionText, opts => opts.MapFrom(src => src.NewOptionText))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
