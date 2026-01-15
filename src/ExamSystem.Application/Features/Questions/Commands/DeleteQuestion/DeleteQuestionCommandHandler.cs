using ExamSystem.Application.Common.Results;
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
            var examExist = await _unitOfWork.Repository<Exam>().AnyAsync(x => x.Id == request.ExamId);
            if (!examExist)
                return Result.Fail(Error.NotFound("ExamNotFound", "Exam with id not found"));

            var question = await _unitOfWork.Repository<Question>().FindAsync(request.QuestionId);
            if (question == null || question.ExamId != request.ExamId)
                return Result.Fail(Error.NotFound("QuestionNotFound", "Question with id not found in this exam"));

            _unitOfWork.Repository<Question>().Remove(question);
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok("Question deleted successfully");
        }
    }
}
