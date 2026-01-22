using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Features.Questions.Shared;
using ExamSystem.Domain.Entities.Questions;
using ExamSystem.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Application.Features.Questions.Queries.GetQuestionsByExamId
{
    public class GetQuestionsByExamIdQueryHandler : IRequestHandler<GetQuestionsByExamIdQuery, Result<PaginatedResult<QuestionResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetQuestionsByExamIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<PaginatedResult<QuestionResponse>>> Handle(GetQuestionsByExamIdQuery request, CancellationToken cancellationToken)
        {
            var questionRepo = _unitOfWork.Repository<Question>();
            var questionWithPaginatedQuery = questionRepo.GetAsQuery().AsNoTracking()
                                .Where(x => x.ExamId == request.ExamId && x.Exam.DoctorId == request.DoctorId)
                                .OrderBy(x => x.QuestionType).ThenBy(x => x.Id)
                                .Skip((request.PageNumber - 1) * request.PageSize)
                                .Take(request.PageSize);

            var questionsDto = await questionWithPaginatedQuery.ProjectTo<QuestionResponse>(_mapper.ConfigurationProvider)
                                     .ToListAsync(cancellationToken);

            var totalCount = await questionRepo
                    .CountAsync(x => x.ExamId == request.ExamId && x.Exam.DoctorId == request.DoctorId, cancellationToken);

            return PaginatedResult<QuestionResponse>
                    .CreatePaginatedResult(questionsDto, totalCount, request.PageNumber, request.PageSize, request.BaseUrl, request.QueryParams);
        }

    }
}
