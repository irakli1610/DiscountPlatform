//// Copyright (C) TBC Bank. All Rights Reserved.

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace offers.itacademy.ge.API.Infrastructure.Healthchecks
{
    public class MvcHealthCheck : IHealthCheck
    {
        private readonly HttpClient _httpClient;

        public MvcHealthCheck(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7161/ping", cancellationToken).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    return HealthCheckResult.Healthy("MVC is running.");
                }
                return HealthCheckResult.Unhealthy("MVC is not responding.");
            }
            catch
            {
                return HealthCheckResult.Unhealthy("MVC is unreachable.");
            }
        }
    }
}
