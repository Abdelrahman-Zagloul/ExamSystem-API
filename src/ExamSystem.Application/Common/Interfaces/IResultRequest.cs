using ExamSystem.Application.Common.Results;
using MediatR;

namespace ExamSystem.Application.Common.Interfaces
{
    public interface IResultRequest<TResponse> : IRequest<Result<TResponse>>
    {
    }
}
