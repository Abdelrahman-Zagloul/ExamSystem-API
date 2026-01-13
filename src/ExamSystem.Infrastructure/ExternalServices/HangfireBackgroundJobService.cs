using ExamSystem.Application.Contracts.ExternalServices;
using Hangfire;
using System.Linq.Expressions;

namespace ExamSystem.Infrastructure.ExternalServices
{
    internal class HangfireBackgroundJobService : IBackgroundJobService
    {
        public string Enqueue(Expression<Action> methodCall) => BackgroundJob.Enqueue(methodCall);
        public string Schedule(Expression<Action> methodCall, TimeSpan delay) => BackgroundJob.Schedule(methodCall, delay);

        public string Enqueue<T>(Expression<Func<T, Task>> methodCall) => BackgroundJob.Enqueue<T>(methodCall);
        public string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay) => BackgroundJob.Schedule<T>(methodCall, delay);
    }
}

