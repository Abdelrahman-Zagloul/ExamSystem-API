using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Features.Questions.Shared;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Entities.Questions;
using ExamSystem.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Application.Features.Questions.Queries.GetQuestionById
{
    public class GetQuestionByIdQueryHandler : IRequestHandler<GetQuestionByIdQuery, Result<QuestionResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetQuestionByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<QuestionResponse>> Handle(GetQuestionByIdQuery request, CancellationToken cancellationToken)
        {
            var exam = await _unitOfWork.Repository<Exam>().FindAsync(cancellationToken, request.ExamId);
            if (exam == null)
                return Error.NotFound("ExamNotFound", "Exam with this id not found");

            if (exam.DoctorId != request.DoctorId)
                return Error.Forbidden("AccessDenied", "You don't have permission to access this exam");

            var questionResponse = await _unitOfWork.Repository<Question>()
                .GetAsQuery(true).Where(x => x.Id == request.QuestionId)
                .ProjectTo<QuestionResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
            if (questionResponse == null)
                return Error.NotFound("QuestionNotFound", "Question With this id not found");

            return questionResponse;
        }
    }
}
