using ExamSystem.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ExamSystem.API.HealthChecks
{
    public class QuestionsHealthCheck : IHealthCheck
    {
        private readonly ExamDbContext _context;

        public QuestionsHealthCheck(ExamDbContext context)
        {
            _context = context;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var hasQuestion = await _context.Questions.AsNoTracking()
                    .Take(1).AnyAsync(cancellationToken);

                return hasQuestion
                    ? HealthCheckResult.Healthy("Questions Query is working")
                    : HealthCheckResult.Degraded("No questions found");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Questions Query failed", ex);
            }
        }
    }
}
