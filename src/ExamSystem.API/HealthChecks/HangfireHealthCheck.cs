using Hangfire;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ExamSystem.API.HealthChecks
{
    public class HangfireHealthCheck : IHealthCheck
    {
        private readonly JobStorage _jobStorage;
        public HangfireHealthCheck(JobStorage jobStorage)
        {
            _jobStorage = jobStorage;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var monitoringApi = _jobStorage.GetMonitoringApi();
                var servers = monitoringApi.Servers();

                if (servers == null || servers.Count == 0)
                    return Task.FromResult(HealthCheckResult.Degraded("Hangfire servers unavailable"));

                return Task.FromResult(HealthCheckResult.Healthy($"Hangfire servers: {servers.Count}"));
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Degraded("Hangfire is unavailable", ex));
            }
        }
    }
}

