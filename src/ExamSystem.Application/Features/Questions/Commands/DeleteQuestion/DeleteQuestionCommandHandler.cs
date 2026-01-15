using ExamSystem.Application.Common.Results;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.Interfaces;
using MediatR;

namespace ExamSystem.Application.Features.Questions.Commands.DeleteQuestion
{
    public class DeleteQuestionCommandHandler : IRequestHandler<DeleteQuestionCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteQuestionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<string>> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
        {
            var examExist = await _unitOfWork.Repository<Exam>().AnyAsync(x => x.Id == request.ExamId);
            if (!examExist)
                return Result<string>.Fail(Error.NotFound("ExamNotFound", "Exam with id not found"));

            var question = await _unitOfWork.Repository<Question>().FindAsync(request.QuestionId);
            if (question == null || question.ExamId != request.ExamId)
                return Result<string>.Fail(Error.NotFound("QuestionNotFound", "Question with id not found in this exam"));

            _unitOfWork.Repository<Question>().Remove(question);
            await _unitOfWork.SaveChangesAsync();
            return Result<string>.Ok("Question deleted successfully");
        }
    }
}
