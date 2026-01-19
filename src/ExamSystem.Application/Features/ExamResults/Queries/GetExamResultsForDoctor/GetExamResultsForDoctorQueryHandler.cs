using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExamSystem.Application.Common.Extensions;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Features.ExamResults.DTOs;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Application.Features.ExamResults.Queries.GetExamResultsForDoctor
{
    public class GetExamResultsForDoctorQueryHandler
        : IRequestHandler<GetExamResultsForDoctorQuery, Result<PaginatedResult<StudentExamResultForDoctorDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetExamResultsForDoctorQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PaginatedResult<StudentExamResultForDoctorDto>>> Handle(GetExamResultsForDoctorQuery request, CancellationToken cancellationToken)
        {
            var exam = await _unitOfWork.Repository<Exam>().FindAsync(cancellationToken, request.ExamId);
            if (exam == null)
                return Error.NotFound("ExamNotFound", "Exam with this id not found");

            if (exam.DoctorId != request.DoctorId)
                return Error.Forbidden("AccessDenied", "You don't have permission to access this exam");

            if (exam.EndAt > DateTime.UtcNow)
                return Error.Conflict("ExamNotFinished", "Exam not finished to get this result");


            var examResultsQuery = _unitOfWork.Repository<ExamResult>()
                .GetAsQuery(true).Where(x => x.ExamId == request.ExamId)
                .ApplyExamResultStatusFilter(request.Status);

            var totalCount = await examResultsQuery.CountAsync(cancellationToken);

            var examResultsResponse = await examResultsQuery
                .ProjectTo<StudentExamResultForDoctorDto>(_mapper.ConfigurationProvider)
                .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return PaginatedResult<StudentExamResultForDoctorDto>
                .CreatePaginatedResult(examResultsResponse,
                                       totalCount,
                                       request.PageNumber,
                                       request.PageSize,
                                       request.BaseUrl,
                                       request.QueryParams);
        }
    }
}
