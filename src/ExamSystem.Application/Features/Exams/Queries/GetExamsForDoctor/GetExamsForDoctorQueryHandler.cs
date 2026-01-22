using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Features.Exams.Queries.GetExamsForDoctor.Responses;
using ExamSystem.Application.Features.Exams.Shared;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Application.Features.Exams.Queries.GetExamsForDoctor
{
    public class GetExamsForDoctorQueryHandler : IRequestHandler<GetExamsForDoctorQuery, Result<PaginatedResult<ExamSummaryResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetExamsForDoctorQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PaginatedResult<ExamSummaryResponse>>> Handle(GetExamsForDoctorQuery request, CancellationToken cancellationToken)
        {
            var examQuery = _unitOfWork.Repository<Exam>().GetAsQuery(true)
                    .Where(x => x.DoctorId == request.DoctorId)
                    .ApplyExamStatusFilter(request.ExamStatus);

            var totalCount = await examQuery.CountAsync(cancellationToken);

            var examsForDoctorDto = await examQuery
                        .OrderBy(x => x.Id)
                        .ProjectTo<ExamSummaryResponse>(_mapper.ConfigurationProvider)
                        .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
                        .ToListAsync(cancellationToken);

            return PaginatedResult<ExamSummaryResponse>
                .CreatePaginatedResult(examsForDoctorDto, totalCount, request.PageNumber, request.PageSize, request.BaseUrl, request.QueryParams);
        }
    }
}