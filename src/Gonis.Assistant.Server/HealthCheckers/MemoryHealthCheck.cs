using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Gonis.Assistant.Server.HealthCheckers
{
    public class MemoryHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var threshold = 3L * 1024L * 1024L * 1024L;

            var allocated = GC.GetTotalMemory(false);
            var data = new Dictionary<string, object>
            {
                { "AllocatedBytes", allocated },
                { "AllocatedKBytes", allocated / 1024 },
                { "AllocatedMBytes", allocated / 1024 / 1024 },
                { "AllocatedGBytes", allocated / 1024 / 1024 / 1024 },
                { "Gen0Collections", GC.CollectionCount(0) },
                { "Gen1Collections", GC.CollectionCount(1) },
                { "Gen2Collections", GC.CollectionCount(2) }
            };

            var status = allocated < threshold ? HealthStatus.Healthy : HealthStatus.Unhealthy;

            return Task.FromResult(new HealthCheckResult(
                status,
                "Notify when memory " +
                $">= {threshold} bites.",
                null,
                data));
        }
    }
}
