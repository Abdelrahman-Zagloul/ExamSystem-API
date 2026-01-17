using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Features.Questions.DTOs;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Application.Features.Questions.Queries.GetAllQuestionsForDoctor
{
    public class GetExamQuestionsForDoctorQueryHandler : IRequestHandler<GetExamQuestionsForDoctorQuery, Result<PaginatedResult<QuestionForDoctorDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetExamQuestionsForDoctorQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<PaginatedResult<QuestionForDoctorDto>>> Handle(GetExamQuestionsForDoctorQuery request, CancellationToken cancellationToken)
        {
            var questionRepo = _unitOfWork.Repository<Question>();
            var questionWithPaginatedQuery = questionRepo.GetAsQuery().AsNoTracking()
                                .Where(x => x.ExamId == request.ExamId && x.Exam.DoctorId == request.DoctorId)
                                .OrderBy(x => x.QuestionType).ThenBy(x => x.Id)
                                .Skip((request.PageNumber - 1) * request.PageSize)
                                .Take(request.PageSize);

            var questionsDto = await questionWithPaginatedQuery.ProjectTo<QuestionForDoctorDto>(_mapper.ConfigurationProvider)
                                     .ToListAsync();

            var totalCount = await questionRepo
                    .CountAsync(x => x.ExamId == request.ExamId && x.Exam.DoctorId == request.DoctorId);

            if (!questionsDto.Any())
                return Error.NotFound("QuestionNotFound", "No Question Found For this exam with existing doctor"); // use Implicit conversions for result

            return PaginatedResult<QuestionForDoctorDto>
                    .CreatePaginatedResult(questionsDto, totalCount, request.PageNumber, request.PageSize, request.BaseUrl);
        }

    }
}
