using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Entities.Questions;
using ExamSystem.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Application.Features.Questions.Commands.UpdateQuestion
{
    public class UpdateQuestionCommandHandler : IRequestHandler<UpdateQuestionCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateQuestionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
        {
            var exam = await _unitOfWork.Repository<Exam>().FindAsync(cancellationToken, request.ExamId);
            if (exam == null)
                return Error.NotFound("ExamNotFound", "Exam with id not found");

            if (exam.StartAt <= DateTime.UtcNow)
                return Error.BadRequest("ExamStarted", "Can't update question because the exam has started");

            var question = await _unitOfWork.Repository<Question>().GetAsQuery(false)
                .Where(x => x.Id == request.QuestionId && x.ExamId == request.ExamId)
                .Include(x => x.Options).FirstOrDefaultAsync(cancellationToken);

            if (question == null)
                return Error.NotFound("QuestionNotFound", "Question with id not found in this exam");

            if (request.QuestionText != null)
                question.QuestionText = request.QuestionText;
            if (request.NewQuestionMark.HasValue)
                question.QuestionMark = request.NewQuestionMark.Value;

            if (request.Options != null && request.Options.Any())
            {
                var optionIds = question.Options.Select(o => o.Id).ToHashSet();
                foreach (var optionDto in request.Options)
                {
                    if (!optionIds.Contains(optionDto.OptionId))
                        return Error.NotFound("OptionNotFound", $"Option with id {optionDto.OptionId} not found");
                }

                foreach (var optionDto in request.Options)
                {
                    var option = question.Options.First(o => o.Id == optionDto.OptionId);
                    if (optionDto.NewOptionText != null)
                        option.OptionText = optionDto.NewOptionText;
                }
            }
            if (request.NewCorrectOptionId.HasValue)
            {
                var optionExist = question.Options.Any(x => x.Id == request.NewCorrectOptionId.Value);
                if (!optionExist)
                    return Error.NotFound("CorrectOptionNotFound", "Correct option does not exist in this question");
                question.CorrectOptionId = request.NewCorrectOptionId.Value;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok("Question Updated Successfully");
        }
    }
}
