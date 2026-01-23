using AutoMapper;
using ExamSystem.Application.Features.Exams.Commands.CreateExam;
using ExamSystem.Application.Features.Exams.Commands.StartExam.Responses;
using ExamSystem.Application.Features.Exams.Commands.UpdateExam;
using ExamSystem.Application.Features.Exams.Queries.GetExamByIdForDoctor.Responses;
using ExamSystem.Application.Features.Exams.Queries.GetExamsForDoctor.Responses;
using ExamSystem.Application.Features.Exams.Queries.GetExamsForStudent.Responses;
using ExamSystem.Application.Features.Exams.Shared;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Entities.Questions;

namespace ExamSystem.Application.Features.Exams.Mapping
{
    public class ExamMappingProfile : Profile
    {
        public ExamMappingProfile()
        {
            CreateExamMapper();
            UpdateExamMapper();
            GetExamsForDoctorMapper();
            GetExamByIdForDoctorMapper();
            StartExamMapper();
            GetExamsForStudentMapper();
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
            CreateMap<Exam, ExamSummaryResponse>()
                .ForMember(dest => dest.ExamStatus, opts => opts.MapFrom(ExamExtensions.GetExamStatusExpression()))
                .ForMember(dest => dest.QuestionsCount, opts => opts.MapFrom(src => src.Questions.Count))
                .ForMember(dest => dest.SubmissionsCount, opts => opts.MapFrom(src => src.ExamResults.Count));
        }
        private void GetExamByIdForDoctorMapper()
        {
            CreateMap<Exam, ExamDetailsForDoctorResponse>()
                .ForMember(dest => dest.ExamStatus, opts => opts.MapFrom(ExamExtensions.GetExamStatusExpression()))
                .ForMember(dest => dest.QuestionsCount, opts => opts.MapFrom(src => src.Questions.Count))
                .ForMember(dest => dest.SubmissionsCount, opts => opts.MapFrom(src => src.ExamResults.Count))
                .ForMember(dest => dest.StudentsPassCount, opts => opts.MapFrom(src => src.ExamResults.Count(x => x.Score >= (x.TotalMark / 2))))
                .ForMember(dest => dest.StudentsPassCount, opts => opts.MapFrom(src => src.ExamResults.Count(x => x.Score < (x.TotalMark / 2))));
        }
        private void StartExamMapper()
        {
            CreateMap<Exam, StartExamResponse>()
                .ForMember(dest => dest.ExamId, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.QuestionsCount, opts => opts.MapFrom(src => src.Questions.Count));

            CreateMap<Question, ExamQuestionResponse>()
             .ForMember(dest => dest.QuestionId, opts => opts.MapFrom(src => src.Id));
        }
        private void GetExamsForStudentMapper()
        {
            CreateMap<Exam, ExamDetailsForStudentResponse>()
                .ForMember(dest => dest.DoctorName, opts => opts.MapFrom(src => src.Doctor.FullName))
                .ForMember(dest => dest.ExamStatus, opts => opts.MapFrom(ExamExtensions.GetExamStatusExpression()))
                .ForMember(dest => dest.QuestionsCount, opts => opts.MapFrom(src => src.Questions.Count));
        }
    }

}
