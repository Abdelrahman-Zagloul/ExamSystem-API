using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Contracts.Identity;
using ExamSystem.Domain.Entities;
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
            var exam = await examRepo.FindAsync(request.ExamId);
            if (exam == null)
                return Result.Fail(Error.NotFound("ExamNotFound", "Exam With this id not found"));

            if (_currentUserService.UserId != exam.DoctorId)
                return Result.Fail(Error.Forbidden("AccessDenied", "you don't have permission to remove this exam"));

            examRepo.Remove(exam);
            await _unitOfWork.SaveChangesAsync();

            return Result.Ok("Exam Deleted successfully");
        }
    }
}
