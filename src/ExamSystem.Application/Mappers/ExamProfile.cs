using AutoMapper;
using ExamSystem.Application.Features.Exams.Commands.CreateExam;
using ExamSystem.Domain.Entities;

namespace ExamSystem.Application.Mappers
{
    public class ExamProfile : Profile
    {
        public ExamProfile()
        {
            CreateExamMapper();
        }

        private void CreateExamMapper()
        {
            CreateMap<CreateExamCommand, Exam>();
        }
    }
}
