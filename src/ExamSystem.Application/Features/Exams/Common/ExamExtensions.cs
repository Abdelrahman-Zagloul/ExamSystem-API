using ExamSystem.Application.Features.Exams.DTOs;
using ExamSystem.Domain.Entities.Exams;
using System.Linq.Expressions;

namespace ExamSystem.Application.Features.Exams.Common
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
