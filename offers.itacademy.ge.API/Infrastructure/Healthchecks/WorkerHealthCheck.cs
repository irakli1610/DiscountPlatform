////// Copyright (C) TBC Bank. All Rights Reserved.

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace offers.itacademy.ge.API.Infrastructure.Healthchecks
{
    public class WorkerHealthCheck : IHealthCheck
    {
        private readonly HttpClient _httpClient;

        public WorkerHealthCheck(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5001/");     // Worker’s URL
            _httpClient.Timeout = TimeSpan.FromSeconds(5);
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync("worker-health", cancellationToken).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    return HealthCheckResult.Healthy("Worker is running.");
                }
                return HealthCheckResult.Unhealthy("Worker is not responding.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"Worker ping failed: {ex.Message}");
            }
        }
    }
}
