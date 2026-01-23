using System.Linq.Expressions;

namespace ExamSystem.Application.Contracts.ExternalServices
{
    public interface IBackgroundJobService
    {
        // For non-async methods
        string Enqueue(Expression<Action> methodCall);
        string Schedule(Expression<Action> methodCall, TimeSpan delay);
        string Schedule(Expression<Action> methodCall, DateTimeOffset runAt);

        // For async methods
        string Enqueue<T>(Expression<Func<T, Task>> methodCall);
        string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay);
        string Schedule<T>(Expression<Func<T, Task>> methodCall, DateTimeOffset runAt);
    }
}
