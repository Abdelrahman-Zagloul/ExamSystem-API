using AutoMapper;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Domain.Entities.Questions;
using ExamSystem.Domain.Interfaces;
using MediatR;

namespace ExamSystem.Application.Features.Questions.Commands.UpdateQuestion
{
    public class UpdateQuestionCommandHandler : IRequestHandler<UpdateQuestionCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UpdateQuestionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
        {
            var question = await _unitOfWork.Repository<Question>()
                .GetAsync(x => x.Id == request.QuestionId, cancellationToken, x => x.Options);

            if (question == null || question.ExamId != request.ExamId)
                return Error.NotFound("QuestionNotFound", "Question with id not found in this exam");

            _mapper.Map(request, question);

            if (request.Options != null)
            {
                foreach (var optionDto in request.Options)
                {
                    var option = question.Options.FirstOrDefault(o => o.Id == optionDto.OptionId);
                    if (option == null)
                        return Error.NotFound("OptionNotFound", "Option with id not found");
                    _mapper.Map(optionDto, option);
                }
            }
            if (request.NewCorrectOptionId.HasValue)
            {
                var optionExist = question.Options.Any(x => x.Id == request.NewCorrectOptionId.Value);
                if (optionExist)
                    question.CorrectOptionId = request.NewCorrectOptionId.Value;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok("Question Updated Successfully");
        }
    }
}
