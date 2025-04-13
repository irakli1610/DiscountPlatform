// Copyright (C) TBC Bank. All Rights Reserved.

using Microsoft.Extensions.Diagnostics.HealthChecks;
using offers.itacademy.ge.API.Infrastructure.Healthchecks;

namespace offers.itacademy.ge.API.Infrastructure.Extensions
{
    public static class HealthChechExtensions
    {
        public static void AddhealthCheckExtension(this IServiceCollection services, string connectionString)
        {

            services.AddHealthChecks()
                .AddSqlServer(
                    connectionString: connectionString,
                    name: "Database",
                    failureStatus: HealthStatus.Unhealthy, tags: new[] { "infrastructure" })
                .AddCheck<ApiHealthCheck>("API", tags: new[] { "services" })
                .AddCheck<MvcHealthCheck>("MVC", tags: new[] { "services" })
                .AddCheck<WorkerHealthCheck>("Worker", tags: new[] { "background" })
                .AddCheck("Self", () => HealthCheckResult.Healthy(), tags: new[] { "api" });

            services.AddHealthChecksUI(options =>
            {
                options.SetEvaluationTimeInSeconds(15); // Time between checks
                options.MaximumHistoryEntriesPerEndpoint(60); // Keep last 60 entries
                options.AddHealthCheckEndpoint("Discount Platform API", "/healthz");
            }).AddInMemoryStorage();

        }
    }
}
