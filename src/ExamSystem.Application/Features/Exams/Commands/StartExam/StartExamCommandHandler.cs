using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Contracts.ExternalServices;
using ExamSystem.Application.Features.Exams.Commands.StartExam.Responses;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Application.Features.Exams.Commands.StartExam
{
    public class StartExamCommandHandler : IRequestHandler<StartExamCommand, Result<StartExamResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly IGenericRepository<Exam> _examRepo;
        private readonly IGenericRepository<ExamSession> _sessionRepo;
        public StartExamCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
            _examRepo = _unitOfWork.Repository<Exam>();
            _sessionRepo = _unitOfWork.Repository<ExamSession>();
        }
        public async Task<Result<StartExamResponse>> Handle(StartExamCommand request, CancellationToken cancellationToken)
        {
            var exam = await _examRepo.FindAsync(cancellationToken, request.ExamId);
            if (exam == null)
                return Error.NotFound("ExamNotFound", "Exam with this id not found");

            var now = DateTime.UtcNow;
            if (exam.StartAt > now)
                return Error.Conflict("ExamNotStarted", $"Exam not started yet. will start at {exam.StartAt}");

            if (exam.EndAt < now)
                return Error.Conflict("ExamFinished", "Exam is already finished");


            var session = await _sessionRepo
                    .FindAsync(cancellationToken, request.StudentId, request.ExamId);

            if (session != null)
            {
                if (session.SubmittedAt != null)
                    return Error.Conflict("ExamAlreadySubmitted", "You already submitted this exam");

                return await LoadExamResponseAsync(request, exam.EndAt, cancellationToken);
            }
            var examSession = new ExamSession
            {
                ExamId = request.ExamId,
                StudentId = request.StudentId,
                StartedAt = now
            };

            await _sessionRepo.AddAsync(examSession, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await LoadExamResponseAsync(request, exam.EndAt, cancellationToken);
        }

        private async Task<Result<StartExamResponse>> LoadExamResponseAsync(
            StartExamCommand request, DateTime examEndTime, CancellationToken cancellationToken)
        {
            var cacheKey = $"ExamResponse|{request.ExamId}";
            var cacheExamResponse = await _cacheService.GetAsync<StartExamResponse>(cacheKey);
            if (cacheExamResponse != null)
                return cacheExamResponse with { Questions = ShuffleQuestions(request, cacheExamResponse.Questions) };

            var examResponse = await _examRepo.GetAsQuery(true)
                            .Where(x => x.Id == request.ExamId)
                            .ProjectTo<StartExamResponse>(_mapper.ConfigurationProvider)
                            .FirstOrDefaultAsync(cancellationToken);

            if (examResponse is null)
                return Error.NotFound("ExamNotFound", "Exam not found");
            await _cacheService.SetAsync(cacheKey, examResponse, TimeSpan.FromMinutes((examEndTime - DateTime.UtcNow).TotalMinutes));

            return examResponse with { Questions = ShuffleQuestions(request, examResponse.Questions) };
        }
        private List<ExamQuestionResponse> ShuffleQuestions(StartExamCommand request, IReadOnlyList<ExamQuestionResponse> questions)
        {
            var random = new Random(HashCode.Combine(request.StudentId, request.ExamId));
            var list = questions.ToList();

            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }

            return list;
        }
    }
}
