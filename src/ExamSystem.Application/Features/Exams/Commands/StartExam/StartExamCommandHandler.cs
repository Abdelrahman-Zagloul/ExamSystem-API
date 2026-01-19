using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Features.Exams.DTOs;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Application.Features.Exams.Commands.StartExam
{
    public class StartExamCommandHandler : IRequestHandler<StartExamCommand, Result<StartExamResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StartExamCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<StartExamResponseDto>> Handle(StartExamCommand request, CancellationToken cancellationToken)
        {
            var examRepo = _unitOfWork.Repository<Exam>();

            var exam = await examRepo.FindAsync(cancellationToken, request.ExamId);
            if (exam == null)
                return Error.NotFound("ExamNotFound", "Exam with this id not found");

            var now = DateTime.UtcNow;
            if (exam.StartAt > now)
                return Error.Conflict("ExamNotStarted", $"Exam not started yet. will start at {exam.StartAt}");

            if (exam.EndAt < now)
                return Error.Conflict("ExamFinished", "Exam is already finished");


            var sessionRepo = _unitOfWork.Repository<ExamSession>();
            var session = await sessionRepo
                    .FindAsync(cancellationToken, request.StudentId, request.ExamId);

            if (session != null)
            {
                if (session.SubmittedAt != null)
                    return Error.Conflict("ExamAlreadySubmitted", "You already submitted this exam");
                return Error.Conflict("ExamAlreadyStarted", "You have already started this exam");
            }

            var examSession = new ExamSession
            {
                ExamId = request.ExamId,
                StudentId = request.StudentId,
                StartedAt = now
            };

            await sessionRepo.AddAsync(examSession, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var examResponse = await examRepo.GetAsQuery(true)
                            .Where(x => x.Id == request.ExamId)
                            .ProjectTo<StartExamResponseDto>(_mapper.ConfigurationProvider)
                            .FirstOrDefaultAsync(cancellationToken);

            if (examResponse is null)
                return Error.NotFound("ExamNotFound", "Exam not found");

            return examResponse;
        }
    }
}
