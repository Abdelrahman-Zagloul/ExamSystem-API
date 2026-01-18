using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Features.Exams.DTOs;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Application.Features.Exams.Queries.StartExam
{
    public class StartExamQueryHandler : IRequestHandler<StartExamQuery, Result<StartExamResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StartExamQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<StartExamResponseDto>> Handle(StartExamQuery request, CancellationToken cancellationToken)
        {
            var exam = await _unitOfWork.Repository<Exam>().FindAsync(cancellationToken, request.ExamId);
            if (exam == null)
                return Error.NotFound("ExamNotFound", "Exam with this id not found");

            if (exam.StartAt > DateTime.UtcNow)
                return Error.NotFound("ExamNotStarted", $"Exam not started yet. start at {exam.StartAt}");

            if (exam.EndAt < DateTime.UtcNow)
                return Error.NotFound("ExamFinished", "Exam is already finished");

            var hasSubmittedExam = await _unitOfWork.Repository<ExamResult>()
                    .AnyAsync(x => x.ExamId == request.ExamId && x.StudentId == request.StudentId, cancellationToken);

            if (hasSubmittedExam)
                return Error.NotFound("ExamAlreadySubmitted", "you already submit the exam. You cannot take the exam more than once");


            var examQuery = _unitOfWork.Repository<Exam>()
                            .GetAsQuery(true).Where(x => x.Id == request.ExamId);

            var examResponseDto = await examQuery
                            .ProjectTo<StartExamResponseDto>(_mapper.ConfigurationProvider)
                            .FirstOrDefaultAsync(cancellationToken);
            if (examResponseDto is null)
                return Error.NotFound("ExamNotFound", "Exam not found");

            return examResponseDto;
        }
    }
}
