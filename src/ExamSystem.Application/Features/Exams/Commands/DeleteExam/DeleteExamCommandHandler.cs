using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Contracts.Identity;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Interfaces;
using MediatR;

namespace ExamSystem.Application.Features.Exams.Commands.DeleteExam
{
    public class DeleteExamCommandHandler : IRequestHandler<DeleteExamCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public DeleteExamCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(DeleteExamCommand request, CancellationToken cancellationToken)
        {
            var examRepo = _unitOfWork.Repository<Exam>();
            var exam = await examRepo.FindAsync(cancellationToken, request.ExamId);
            if (exam == null)
                return Error.NotFound("ExamNotFound", "Exam With this id not found");

            if (exam.DoctorId != _currentUserService.UserId!)
                return Error.Forbidden("AccessDenied", "you don't have permission to remove this exam");

            if (DateTime.UtcNow > exam.EndAt)
                return Error.Conflict("ExamAlreadyFinished", "Exam already finished and cannot be deleted.");

            var hasSession = await _unitOfWork.Repository<ExamSession>()
                            .AnyAsync(x => x.ExamId == request.ExamId, cancellationToken);
            if (hasSession)
                return Error.Conflict("ExamHasSessions", "This exam cannot deleted because students already started it.");

            var hasResult = await _unitOfWork.Repository<ExamResult>()
                            .AnyAsync(x => x.ExamId == request.ExamId, cancellationToken);
            if (hasResult)
                return Error.Conflict("ExamHasResults", "This exam cannot be deleted because results already been submitted.");


            examRepo.Remove(exam);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok("Exam Deleted successfully");
        }
    }
}
