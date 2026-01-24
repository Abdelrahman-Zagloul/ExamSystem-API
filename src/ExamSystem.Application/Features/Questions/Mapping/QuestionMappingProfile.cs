using AutoMapper;
using ExamSystem.Application.Features.Questions.Commands.CreateQuestion;
using ExamSystem.Application.Features.Questions.Commands.CreateQuestion.Requests;
using ExamSystem.Application.Features.Questions.Shared;
using ExamSystem.Domain.Entities.Questions;

namespace ExamSystem.Application.Mappers
{
    public class QuestionMappingProfile : Profile
    {
        public QuestionMappingProfile()
        {
            CreateQuestionMapper();
            GetQuestionsMapper();
        }


        private void CreateQuestionMapper()
        {
            CreateMap<CreateQuestionCommand, Question>()
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options))
                .ForMember(dest => dest.CorrectOptionId, opt => opt.Ignore());

            CreateMap<CreateOptionRequest, Option>()
               .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
        private void GetQuestionsMapper()
        {
            CreateMap<Question, QuestionResponse>()
                .ForMember(dest => dest.QuestionId, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.ExamTitle, opts => opts.MapFrom(src => src.Exam.Title));

            CreateMap<Option, OptionResponse>()
                .ForMember(dest => dest.OptionId, opts => opts.MapFrom(src => src.Id));
        }

    }
}
