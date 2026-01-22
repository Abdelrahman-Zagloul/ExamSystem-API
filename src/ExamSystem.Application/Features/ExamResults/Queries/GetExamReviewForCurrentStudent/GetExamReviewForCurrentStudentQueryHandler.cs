using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Features.ExamResults.Queries.GetExamReviewForCurrentStudent.Responses;
using ExamSystem.Application.Features.Questions.Shared;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Application.Features.ExamResults.Queries.GetExamReviewForCurrentStudent
{
    public class GetExamReviewForCurrentStudentQueryHandler
        : IRequestHandler<GetExamReviewForCurrentStudentQuery, Result<ExamReviewResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetExamReviewForCurrentStudentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<ExamReviewResponse>> Handle(GetExamReviewForCurrentStudentQuery request, CancellationToken cancellationToken)
        {
            var exam = await _unitOfWork.Repository<Exam>().FindAsync(cancellationToken, request.ExamId);

            if (exam == null)
                return Error.NotFound("ExamNotFound", "Exam with this id not found");

            if (exam.EndAt > DateTime.UtcNow)
                return Error.Conflict("ExamNotFinished", "Exam with this id not Finished");

            var examResultQuery = _unitOfWork.Repository<ExamResult>().GetAsQuery(true)
                .Where(x => x.ExamId == request.ExamId && x.StudentId == request.StudentId);

            var dto = await examResultQuery
                .ProjectTo<ExamReviewResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (dto == null)
                return Error.NotFound("ExamResultNotFound", "Exam Result for this student & exam id not found");

            var SelectedOptionIds = await _unitOfWork.Repository<StudentAnswer>().GetAsQuery(true)
                .Where(x => x.ExamId == request.ExamId && x.StudentId == request.StudentId)
                .Select(y => new { y.SelectedOptionId, y.QuestionId })
                .ToListAsync(cancellationToken);

            foreach (var question in dto.Questions)
            {
                var optionId = SelectedOptionIds.FirstOrDefault(x => x.QuestionId == question.QuestionId)?.SelectedOptionId ?? 0;
                question.StudentOption = new OptionResponse
                {
                    OptionId = optionId,
                    OptionText = question.Options.FirstOrDefault(x => x.OptionId == optionId)?.OptionText,
                };
            }

            return dto;
        }

    }
}
