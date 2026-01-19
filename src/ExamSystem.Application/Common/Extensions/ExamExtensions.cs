using ExamSystem.Domain.Entities.Exams;
using System.Linq.Expressions;

namespace ExamSystem.Application.Common.Extensions
{
    public static class ExamExtensions
    {
        public static IQueryable<Exam> ApplyExamStatusFilter(this IQueryable<Exam> examQuery, ExamStatus? examStatus)
        {
            if (!examStatus.HasValue)
                return examQuery;

            var now = DateTime.UtcNow;

            return examStatus.Value switch
            {
                ExamStatus.Upcoming => examQuery.Where(x => x.StartAt > now),
                ExamStatus.Active => examQuery.Where(x => x.StartAt <= now && x.EndAt >= now),
                ExamStatus.Finished => examQuery.Where(x => x.EndAt < now),
                _ => examQuery
            };
        }
        public static IQueryable<ExamResult> ApplyExamResultStatusFilter(this IQueryable<ExamResult> examResultQuery, ExamResultStatus? status)
        {
            if (!status.HasValue)
                return examResultQuery;

            return status.Value switch
            {
                ExamResultStatus.Passed => examResultQuery.Where(x => (x.TotalMark * 0.5) <= x.Score),
                ExamResultStatus.Failed => examResultQuery.Where(x => (x.TotalMark * 0.5) > x.Score),
                _ => examResultQuery
            };
        }

        public static Expression<Func<Exam, ExamStatus>> GetExamStatusExpression()
        {
            return x =>
                x.StartAt > DateTime.UtcNow
                    ? ExamStatus.Upcoming
                    : x.StartAt < DateTime.UtcNow && x.EndAt > DateTime.UtcNow
                    ? ExamStatus.Active
                    : ExamStatus.Finished;
        }
    }
}
