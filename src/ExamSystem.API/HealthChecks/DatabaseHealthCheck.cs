using ExamSystem.Infrastructure.Persistence.Contexts;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ExamSystem.API.HealthChecks
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly ExamDbContext _context;

        public DatabaseHealthCheck(ExamDbContext context)
        {
            _context = context;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync(cancellationToken);

                if (canConnect)
                    return HealthCheckResult.Healthy("Database is reachable");
                return HealthCheckResult.Unhealthy("Cannot connect to database");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Database health check failed", ex);
            }
        }
    }
}
