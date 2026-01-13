using MediatR;

namespace ExamSystem.Application.Common.Results
{
    public interface IResultRequest<TResponse> : IRequest<Result<TResponse>>
    {
    }
}
