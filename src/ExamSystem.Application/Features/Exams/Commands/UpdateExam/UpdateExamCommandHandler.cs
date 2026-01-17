using AutoMapper;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Contracts.Identity;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.Interfaces;
using MediatR;

namespace ExamSystem.Application.Features.Exams.Commands.UpdateExam
{
    public class UpdateExamCommandHandler : IRequestHandler<UpdateExamCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public UpdateExamCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        public async Task<Result> Handle(UpdateExamCommand request, CancellationToken cancellationToken)
        {
            var exam = await _unitOfWork.Repository<Exam>().FindAsync(cancellationToken, request.ExamId);
            if (exam == null)
                return Result.Fail(Error.NotFound());

            if (exam.DoctorId != _currentUserService.UserId)
                return Result.Fail(Error.Forbidden(description: "You don't have permission to update this exam"));

            var validationResult = ValidateExamSchedule(request, exam);
            if (!validationResult.IsSuccess)
                return validationResult;

            _mapper.Map(request, exam);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok("Exam Updated Successfully");
        }

        private Result ValidateExamSchedule(UpdateExamCommand request, Exam exam)
        {
            var startAt = request.StartAt ?? exam.StartAt;
            var endAt = request.EndAt ?? exam.EndAt;

            if (endAt <= startAt)
                return Result.Fail(Error.Validation(description: "End time must be after start time"));

            var availableMinutes = (endAt - startAt).TotalMinutes;
            if (request.DurationInMinutes.HasValue && request.DurationInMinutes.Value > availableMinutes)
                return Result.Fail(Error.Validation(description: $"Duration In Minutes of exam must be less than or equal to {availableMinutes}"));

            return Result.Ok();
        }
    }
}
