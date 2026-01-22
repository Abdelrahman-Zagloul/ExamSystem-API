using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Features.Questions.Queries.GetExamQuestionsForDoctor.Responses;
using ExamSystem.Domain.Entities.Questions;
using ExamSystem.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Application.Features.Questions.Queries.GetAllQuestionsForDoctor
{
    public class GetExamQuestionsForDoctorQueryHandler : IRequestHandler<GetExamQuestionsForDoctorQuery, Result<PaginatedResult<QuestionForDoctorResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetExamQuestionsForDoctorQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<PaginatedResult<QuestionForDoctorResponse>>> Handle(GetExamQuestionsForDoctorQuery request, CancellationToken cancellationToken)
        {
            var questionRepo = _unitOfWork.Repository<Question>();
            var questionWithPaginatedQuery = questionRepo.GetAsQuery().AsNoTracking()
                                .Where(x => x.ExamId == request.ExamId && x.Exam.DoctorId == request.DoctorId)
                                .OrderBy(x => x.QuestionType).ThenBy(x => x.Id)
                                .Skip((request.PageNumber - 1) * request.PageSize)
                                .Take(request.PageSize);

            var questionsDto = await questionWithPaginatedQuery.ProjectTo<QuestionForDoctorResponse>(_mapper.ConfigurationProvider)
                                     .ToListAsync(cancellationToken);

            var totalCount = await questionRepo
                    .CountAsync(x => x.ExamId == request.ExamId && x.Exam.DoctorId == request.DoctorId, cancellationToken);

            return PaginatedResult<QuestionForDoctorResponse>
                    .CreatePaginatedResult(questionsDto, totalCount, request.PageNumber, request.PageSize, request.BaseUrl, request.QueryParams);
        }

    }
}
