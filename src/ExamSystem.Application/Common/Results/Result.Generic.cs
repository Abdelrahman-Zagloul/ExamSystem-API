using ExamSystem.Application.Common.Results.Errors;

namespace ExamSystem.Application.Common.Results
{
    public class Result<TValue> : Result
    {
        private readonly TValue _value;
        public TValue Value => IsSuccess ?
            _value : throw new InvalidOperationException("The value of a failure result can not be accessed.");


        private Result(TValue value) : base()
        {
            _value = value;
        }
        private Result(TValue value, string message) : base(message)
        {
            _value = value;
        }
        private Result(Error error) : base(error)
        {
            _value = default!;
        }
        private Result(List<Error> errors) : base(errors)
        {
            _value = default!;
        }


        public static Result<TValue> Ok(TValue value) => new Result<TValue>(value);
        public static Result<TValue> Ok(TValue value, string message) => new Result<TValue>(value, message);
        public new static Result<TValue> Fail(Error error) => new Result<TValue>(error);
        public new static Result<TValue> Fail(List<Error> errors) => new Result<TValue>(errors);


        // Implicit conversions
        public static implicit operator Result<TValue>(TValue value) => Ok(value);
        public static implicit operator Result<TValue>(Error error) => Fail(error);
        public static implicit operator Result<TValue>(List<Error> errors) => Fail(errors);
    }
}
