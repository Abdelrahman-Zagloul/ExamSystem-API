using ExamSystem.Application.Common.Interfaces;
using FluentValidation;

namespace ExamSystem.Application.Common.Validations
{
    public class PaginationValidator<T> : AbstractValidator<T> where T : IPaginatedQuery
    {
        public PaginationValidator()
        {

            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithMessage("Page size must be greater than 0.")
                .LessThanOrEqualTo(50)
                .WithMessage("Page size must be less than 50");

            RuleFor(x => x.BaseUrl)
              .NotEmpty().WithMessage("BaseUrl is required.")
              .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
              .WithMessage("BaseUrl must be a valid absolute URL.");
        }

    }
}
