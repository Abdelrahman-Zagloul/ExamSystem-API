using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Features.Exams.DTOs;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Application.Features.Exams.Queries.GetExamsForDoctor
{
    public class GetExamsForDoctorQueryHandler : IRequestHandler<GetExamsForDoctorQuery, Result<PaginatedResult<ExamSummaryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetExamsForDoctorQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PaginatedResult<ExamSummaryDto>>> Handle(GetExamsForDoctorQuery request, CancellationToken cancellationToken)
        {
            var examQuery = _unitOfWork.Repository<Exam>().GetAsQuery(true)
                    .Where(x => x.DoctorId == request.DoctorId);

            examQuery = ApplyExamStatusFilter(examQuery, request.ExamStatus);

            var totalCount = await examQuery.CountAsync(cancellationToken);

            var examsForDoctorDto = await examQuery
                        .OrderBy(x => x.Id)
                        .ProjectTo<ExamSummaryDto>(_mapper.ConfigurationProvider)
                        .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
                        .ToListAsync(cancellationToken);

            return PaginatedResult<ExamSummaryDto>
                .CreatePaginatedResult(examsForDoctorDto, totalCount, request.PageNumber, request.PageSize, request.BaseUrl);
        }

        private IQueryable<Exam> ApplyExamStatusFilter(IQueryable<Exam> examQuery, ExamStatus? examStatus)
        {
            if (!examStatus.HasValue)
                return examQuery;

            var now = DateTime.UtcNow;

            return examStatus.Value switch
            {
                ExamStatus.Upcoming => examQuery.Where(x => x.StartAt > now),
                ExamStatus.Active => examQuery.Where(x => x.StartAt <= now && x.EndAt >= now),
                ExamStatus.Finished => examQuery.Where(x => x.EndAt < now),
                _ => examQuery
            };
        }
    }
}