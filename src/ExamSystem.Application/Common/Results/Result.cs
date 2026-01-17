using ExamSystem.Application.Common.Results.Errors;

namespace ExamSystem.Application.Common.Results
{
    public class Result
    {
        private readonly List<Error> _errors = new();

        public IReadOnlyList<Error> Errors => _errors.AsReadOnly();
        public bool IsSuccess => !_errors.Any();
        public string? Message { get; set; }
        protected Result() { }
        protected Result(string message)
        {
            Message = message;
        }
        protected Result(Error error)
        {
            _errors.Add(error);
        }
        protected Result(List<Error> errors)
        {
            _errors.AddRange(errors);
        }


        public static Result Ok() => new Result();
        public static Result Ok(string message) => new Result(message);
        public static Result Fail(Error error) => new Result(error);
        public static Result Fail(List<Error> errors) => new Result(errors);
    }
}
