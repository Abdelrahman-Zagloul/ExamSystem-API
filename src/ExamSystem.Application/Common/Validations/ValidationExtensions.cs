using FluentValidation;

namespace ExamSystem.Application.Common.Validations
{
    public static class ValidationExtensions
    {
        public static IRuleBuilderOptions<T, int> MustBePositiveNumber<T>(
                    this IRuleBuilder<T, int> ruleBuilder, string fieldName)
        {
            return ruleBuilder
                .NotEmpty().WithMessage($"{fieldName} is required.")
                .GreaterThan(0).WithMessage($"{fieldName} must be a positive number.");
        }

    }
}
