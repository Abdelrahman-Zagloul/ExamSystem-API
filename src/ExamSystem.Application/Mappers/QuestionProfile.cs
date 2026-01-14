using AutoMapper;
using ExamSystem.Application.Features.Questions.Commands.CreateQuestion;
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
        }
    }
}
