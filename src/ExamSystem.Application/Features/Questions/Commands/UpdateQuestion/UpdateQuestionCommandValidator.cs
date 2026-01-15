using ExamSystem.Application.Features.Questions.DTOs;
using FluentValidation;

namespace ExamSystem.Application.Features.Questions.Commands.UpdateQuestion
{
    public class UpdateQuestionCommandValidator : AbstractValidator<UpdateQuestionCommand>
    {
        public UpdateQuestionCommandValidator()
        {
            RuleFor(x => x.ExamId)
                .GreaterThan(0)
                .WithMessage("ExamId must be greater than 0");

            RuleFor(x => x.QuestionId)
                .GreaterThan(0)
                .WithMessage("QuestionId must be greater than 0");

            RuleFor(x => x.QuestionText)
               .MaximumLength(1000)
               .When(x => !string.IsNullOrEmpty(x.QuestionText))
               .WithMessage("Question text cannot exceed 1000 characters");

            RuleFor(x => x.NewQuestionMark)
                .GreaterThanOrEqualTo(0)
                .When(x => x.NewQuestionMark.HasValue)
                .WithMessage("Question mark must be greater than or equal to 0");

            RuleForEach(x => x.OptionsDto)
                 .NotNull()
                 .WithMessage("OptionDto cannot be null")
                 .SetValidator(new UpdateOptionDtoValidator()!)
                 .When(x => x.OptionsDto != null);

            RuleFor(x => x.NewCorrectOptionId)
               .GreaterThan(0)
               .When(x => x.NewCorrectOptionId.HasValue)
               .WithMessage("NewCorrectOptionId must be greater than 0");
        }


        private class UpdateOptionDtoValidator : AbstractValidator<UpdateOptionDto>
        {
            public UpdateOptionDtoValidator()
            {
                RuleFor(x => x.OptionId)
                    .GreaterThan(0)
                    .WithMessage("OptionId must be greater than 0");

                RuleFor(x => x.NewOptionText)
                    .MaximumLength(500)
                    .When(x => !string.IsNullOrEmpty(x.NewOptionText))
                    .WithMessage("Option text cannot exceed 500 characters");
            }
        }
    }
}