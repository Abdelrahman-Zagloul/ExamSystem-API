using AutoMapper;
using ExamSystem.Application.Common.Results;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.Interfaces;
using MediatR;

namespace ExamSystem.Application.Features.Questions.Commands.CreateQuestion
{
    public class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CreateQuestionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<string>> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
        {
            var examExist = await _unitOfWork.Repository<Exam>().AnyAsync(e => e.Id == request.ExamId);
            if (!examExist)
                return Result<string>.Fail(Error.NotFound("ExamNotFound", "Exam with id not found"));

            var question = _mapper.Map<Question>(request);

            await _unitOfWork.Repository<Question>().AddAsync(question);
            await _unitOfWork.SaveChangesAsync();
            question.CorrectOptionId = question.Options.ElementAt(request.CorrectOptionNumber - 1).Id;
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Ok("Question created successfully");
        }
    }
}
