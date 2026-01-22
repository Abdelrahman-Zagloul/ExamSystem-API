using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Features.ExamResults.Queries.GetExamResultsForCurrentStudent.Responses;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Application.Features.ExamResults.Queries.GetExamResultsForCurrentStudent
{
    public class GetExamResultsForCurrentStudentQueryHandler : IRequestHandler<GetExamResultsForCurrentStudentQuery, Result<PaginatedResult<StudentExamResultForStudentResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetExamResultsForCurrentStudentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PaginatedResult<StudentExamResultForStudentResponse>>> Handle(GetExamResultsForCurrentStudentQuery request, CancellationToken cancellationToken)
        {
            var examResultsQuery = _unitOfWork.Repository<ExamResult>()
                .GetAsQuery(true)
                .Where(x => x.StudentId == request.StudentId && x.Exam.EndAt < DateTime.UtcNow);

            var totalCount = await examResultsQuery.CountAsync(cancellationToken);

            var dto = await examResultsQuery
                .ProjectTo<StudentExamResultForStudentResponse>(_mapper.ConfigurationProvider)
                .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return PaginatedResult<StudentExamResultForStudentResponse>
                .CreatePaginatedResult(dto,
                                       totalCount,
                                       request.PageNumber,
                                       request.PageSize,
                                       request.BaseUrl,
                                       request.QueryParams);
        }
    }
}
