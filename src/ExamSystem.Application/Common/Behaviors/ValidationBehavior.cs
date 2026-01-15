using ExamSystem.Application.Common.Results;
using FluentValidation;
using MediatR;

namespace ExamSystem.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
     where TRequest : IRequest<TResponse>

    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);

            var failures = (await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken))))
                .SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Any())
            {
                var errors = failures
                    .Select(f => Error.Validation(f.PropertyName, f.ErrorMessage))
                    .ToList();

                return GenerateFailResult(errors);
            }

            return await next();
        }

        private TResponse GenerateFailResult(List<Error> errors)
        {
            var responseType = typeof(TResponse);
            if (responseType == typeof(Result))
                return (TResponse)(object)Result.Fail(errors);

            var failResult = typeof(Result<>)
                    .MakeGenericType(responseType.GenericTypeArguments[0])
                    .GetMethod("Fail", [typeof(List<Error>)])!
                    .Invoke(null, [errors]);

            return (TResponse)failResult!;
        }
    }
}

