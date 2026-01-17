using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.Interfaces;
using MediatR;

namespace ExamSystem.Application.Features.Questions.Commands.DeleteQuestion
{
    public class DeleteQuestionCommandHandler : IRequestHandler<DeleteQuestionCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteQuestionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
        {
            var questionRepo = _unitOfWork.Repository<Question>();
            var question = await questionRepo.FindAsync(cancellationToken, request.QuestionId);
            if (question == null || question.ExamId != request.ExamId)
                return Result.Fail(Error.NotFound("QuestionNotFound", "Question not found in this exam"));

            questionRepo.Remove(question);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok("Question deleted successfully");
        }
    }
}
