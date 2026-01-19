using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Features.Exams.Common;
using ExamSystem.Application.Features.Exams.DTOs;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Application.Features.Exams.Queries.GetExamsForStudent
{
    public class GetExamsForStudentQueryHandler
        : IRequestHandler<GetExamsForStudentQuery, Result<PaginatedResult<ExamDetailsForStudentDto>>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetExamsForStudentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PaginatedResult<ExamDetailsForStudentDto>>> Handle(GetExamsForStudentQuery request, CancellationToken cancellationToken)
        {
            var examQuery = _unitOfWork.Repository<Exam>().GetAsQuery(true)
                    .ApplyExamStatusFilter(request.ExamStatus);

            var totalCount = await examQuery.CountAsync(cancellationToken);

            var examsForStudentDto = await examQuery
                        .OrderBy(x => x.Id)
                        .ProjectTo<ExamDetailsForStudentDto>(_mapper.ConfigurationProvider)
                        .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
                        .ToListAsync(cancellationToken);

            return PaginatedResult<ExamDetailsForStudentDto>
                .CreatePaginatedResult(examsForStudentDto, totalCount, request.PageNumber, request.PageSize, request.BaseUrl);
        }

    }
}
