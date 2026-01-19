using AutoMapper;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Contracts.Identity;
using ExamSystem.Domain.Entities.Exams;
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
                return Error.NotFound();

            if (exam.DoctorId != _currentUserService.UserId)
                return Error.Forbidden(description: "You don't have permission to update this exam");

            if (DateTime.UtcNow > exam.StartAt)
                return Error.BadRequest("Exam is already started and cannot be updated.");

            var validationResult = ValidateExamSchedule(request, exam);
            if (!validationResult.IsSuccess)
                return validationResult;

            _mapper.Map(request, exam);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok("Exam Updated Successfully");
        }

        private Result ValidateExamSchedule(UpdateExamCommand request, Exam exam)
        {
            var newStartAt = request.StartAt ?? exam.StartAt;
            var newEndAt = request.EndAt ?? exam.EndAt;

            if (newEndAt <= newStartAt)
                return Error.BadRequest("InvalidSchedule", "End time must be after start time");

            var newAvailableMinutes = (newEndAt - newStartAt).TotalMinutes;
            if (request.DurationInMinutes.HasValue && request.DurationInMinutes.Value > newAvailableMinutes)
                return Error.BadRequest("InvalidDuration", $"Duration In Minutes of exam must be less than or equal to {newAvailableMinutes}");

            return Result.Ok();
        }
    }
}
