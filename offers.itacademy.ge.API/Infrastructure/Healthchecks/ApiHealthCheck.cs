using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace offers.itacademy.ge.API.Infrastructure.Healthchecks
{
    public class ApiHealthCheck : IHealthCheck
    {
        private readonly HttpClient _httpClient;

        public ApiHealthCheck(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7230/api/ping", cancellationToken).ConfigureAwait(false); // relative URI due to BaseAddress
                if (response.IsSuccessStatusCode)
                {
                    return HealthCheckResult.Healthy("API is running.");
                }
                return HealthCheckResult.Unhealthy("API is not responding.");
            }
            catch
            {
                return HealthCheckResult.Unhealthy("API is unreachable.");
            }
        }
    }
}
